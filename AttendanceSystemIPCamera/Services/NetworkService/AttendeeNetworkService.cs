﻿using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using System.Linq;
using AttendanceSystemIPCamera.Framework;
using System.Threading;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public interface IAttendeeNetworkService
    {
        Task<AttendeeViewModel> Login(LoginViewModel loginViewModel);
        public Task<ChangeRequestSimpleViewModel> CreateChangeRequest(CreateChangeRequestViewModel viewModel);
    }

    public class AttendeeNetworkService : IAttendeeNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;
        protected IPAddress supervisorIPAddress
        { get => NetworkUtils.SupervisorAddress; set => NetworkUtils.SupervisorAddress = value; }

        private MyUnitOfWork unitOfWork;
        private ISessionService sessionService;
        private IAttendeeService attendeeService;
        private IGroupService groupService;
        private IMapper mapper;

        private const int MAX_TRY_TIMES = 2;
        private const int TIME_OUT = 20 * 1000;

        private Communicator communicator;

        public AttendeeNetworkService(MyUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.sessionService = unitOfWork.SessionService;
            this.attendeeService = unitOfWork.AttendeeService;
            this.groupService = unitOfWork.GroupService;
            this.mapper = AutoMapperConfiguration.GetInstance();
        }

        public async Task<AttendeeViewModel> Login(LoginViewModel loginViewModel)
        {
            var networkRequest = GetNetworkRequest(NetworkRoute.LOGIN, loginViewModel);

            string reqMess = JsonConvert.SerializeObject(networkRequest);
            object responseData = await SendRequest(reqMess);
            if (responseData == null)
            {
                throw new BaseException(ErrorMessage.NETWORK_ERROR);
            }
            var attendee = await ProcessLoginResponse(responseData);

            if (attendee != null) return attendee;
            throw new BaseException(ErrorMessage.LOGIN_FAIL);
        }

        private NetworkRequest<object> GetNetworkRequest(string route, object request)
        {
            var networkRequest = new NetworkRequest<object>();
            this.GetLocalIpAddress(ref networkRequest);
            networkRequest.Route = route;
            networkRequest.Request = request;
            return networkRequest;
        }

        public async Task<ChangeRequestSimpleViewModel> CreateChangeRequest(CreateChangeRequestViewModel viewModel)
        {
            var networkRequest = GetNetworkRequest(NetworkRoute.CHANGE_REQUEST, viewModel);

            string reqMess = JsonConvert.SerializeObject(networkRequest);
            object responseData = await SendRequest(reqMess);
            if (responseData == null)
            {
                throw new BaseException(ErrorMessage.NETWORK_ERROR);
            }
            var newChangeRequest = JsonConvert.DeserializeObject<ChangeRequestSimpleViewModel>(responseData.ToString());
            if (newChangeRequest != null) return newChangeRequest;
            throw new BaseException(ErrorMessage.CHANGE_REQUEST_FAIL);
        }


        private async Task<object> SendRequest(string message)
        {
            bool isContinue = true;
            int i = 0;
            object responseData = null;
            do
            {
                communicator = GetCommunicator(this.supervisorIPAddress);
                communicator.Send(Encoding.UTF8.GetBytes(message));
                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TIME_OUT);

                    var receiveTask = Task.Run(() =>
                    {
                        object responseData = communicator.Receive();
                        this.supervisorIPAddress = communicator.RemoteHostIPAddress; //save supervisor ip address
                        return responseData;
                    }, cts.Token);

                    var tcs = new TaskCompletionSource<bool>();
                    using (cts.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                    {
                        if (receiveTask != await Task.WhenAny(receiveTask, tcs.Task))
                        {
                            this.localServer.Close();
                            throw new OperationCanceledException(cts.Token);
                        }
                        else
                        {
                            responseData = receiveTask.Result;
                        }
                    }

                    isContinue = false;
                }
                catch (OperationCanceledException)
                {
                    this.supervisorIPAddress = IPAddress.Broadcast; // reset to ip broadcast
                }
                i++;
            } while (isContinue && i < MAX_TRY_TIMES);
            return responseData;
        }

        private Communicator GetCommunicator(IPAddress ipAddress)
        {
            if (localServer == null || localServer?.Client == null)
                localServer = new UdpClient();

            this.supervisorIPAddress = ipAddress;
            this.remoteHostEP = new IPEndPoint(ipAddress, NetworkUtils.RunningPort);
            return new Communicator(localServer, ref this.remoteHostEP);
        }

        private async Task<AttendeeViewModel> ProcessLoginResponse(object responseData)
        {
            //process response
            var attendanceInfo = JsonConvert.DeserializeObject<AttendanceNetworkViewModel>(responseData.ToString());
            if (attendanceInfo != null && attendanceInfo.Success)
            {
                var attendee = await SaveAttendanceAsync(attendanceInfo);
                return attendee;
            }
            return null;
        }

        private async Task<AttendeeViewModel> SaveAttendanceAsync(AttendanceNetworkViewModel attendanceInfo)
        {
            AttendeeViewModel attendee = null;
            using (var scope = unitOfWork.CreateTransaction())
            {
                try
                {
                    var groupVMs = mapper.ProjectTo<GroupNetworkViewModel, GroupViewModel>(attendanceInfo.Groups).ToList();
                    var savedGroups = await groupService.AddGroupIfNotInDbAsync(groupVMs);
                    var groupIds = savedGroups.Select(g => g.Id).ToList();
                    //
                    attendee = await attendeeService.AddAttendeeWithGroupsIfNotInDb(attendanceInfo.AttendeeCode, attendanceInfo.AttendeeName, groupIds);
                    //
                    var sessionNetworkVms = new List<SessionNetworkViewModel>();
                    attendanceInfo.Groups.ForEach(group =>
                    {
                        if (group.Sessions != null && group.Sessions.Count > 0)
                        {
                            group.Sessions.ForEach(session =>
                            {
                                var gId = savedGroups.Where(g => g.Code.Equals(group.Code))
                                                    .Select(g => g.Id)
                                                    .FirstOrDefault();
                                session.GroupId = gId;

                                session.Records.ForEach(record =>
                                {
                                    record.AttendeeId = attendee.Id;
                                });
                            });
                            sessionNetworkVms.AddRange(group.Sessions);
                        }

                    });
                    var sessionVms = mapper.ProjectTo<SessionNetworkViewModel, SessionViewModel>(sessionNetworkVms)
                                            .ToList();
                    await sessionService.AddSessionsWithRecordsAsync(sessionVms, attendee.Id);
                    scope.Commit();
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
            return attendee;
        }
        private void GetLocalIpAddress(ref NetworkRequest<object> networkRequest)
        {
            IPAddress localIp = null;
            IPAddress.TryParse(NetworkUtils.GetLocalIPAddress(), out localIp);
            if (localIp != null)
            {
                networkRequest.IPAddress = localIp.ToString();
            }
            else throw new BaseException(ErrorMessage.CANNOT_GET_LOCAL_IP_ADDRESS);
        }

    }
}
