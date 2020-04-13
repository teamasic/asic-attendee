using AttendanceSystemIPCamera.Framework.ViewModels;
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
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using AttendanceSystemIPCamera.Services.AttendanceService;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public interface IAttendeeNetworkService
    {
        Task<AttendeeViewModel> Refresh(LoginViewModel loginViewModel);
        public Task<ChangeRequestSimpleViewModel> CreateChangeRequest(CreateChangeRequestViewModel viewModel);
    }

    public class AttendeeNetworkService : IAttendeeNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;
        protected IPAddress supervisorIPAddress
        { get => NetworkUtils.SupervisorAddress; set => NetworkUtils.SupervisorAddress = value; }

        private MyUnitOfWork unitOfWork;
        private IAttendanceService attendanceService;

        private const int MAX_TRY_TIMES = 2;
        private const int TIME_OUT = 20 * 1000;

        private Communicator communicator;

        public AttendeeNetworkService(MyUnitOfWork unitOfWork, IAttendanceService attendanceService)
        {
            this.unitOfWork = unitOfWork;
            this.attendanceService = attendanceService;
        }

        public async Task<AttendeeViewModel> Refresh(LoginViewModel loginViewModel)
        {
            //prepare request
            var networkRequest = GetNetworkRequest(NetworkRoute.REFRESH_ATTENDANCE_DATA, loginViewModel);

            string reqMess = JsonConvert.SerializeObject(networkRequest);
            //send request
            object responseData = await SendRequest(reqMess);
            if (responseData == null)
            {
                throw new BaseException(ErrorMessage.NETWORK_ERROR);
            }
            //process response
            var attendanceInfo = JsonConvert.DeserializeObject<AttendanceNetworkViewModel>(responseData.ToString());
            if (attendanceInfo != null && attendanceInfo.Success)
            {
                var attendee = await attendanceService.SaveAttendanceDataAsync(attendanceInfo);
                return attendee;
            }
            throw new BaseException(ErrorMessage.ATTENDEE_NOT_FOUND);
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


        private NetworkRequest<object> GetNetworkRequest(string route, object request)
        {
            var networkRequest = new NetworkRequest<object>();
            this.GetLocalIpAddress(ref networkRequest);
            networkRequest.Route = route;
            networkRequest.Request = request;
            return networkRequest;
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
                    cts.CancelAfter(TIME_OUT);//request timeout after TIME_OUT milisec

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
