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

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public interface IAttendeeNetworkService
    {
        Task<AttendeeViewModel> Start(NetworkMessageViewModel message);
    }

    public class AttendeeNetworkService : IAttendeeNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;

        private MyUnitOfWork unitOfWork;
        private ISessionService sessionService;
        private IAttendeeService attendeeService;
        private IGroupService groupService;
        private IMapper mapper;

        public AttendeeNetworkService(MyUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.sessionService = unitOfWork.SessionService;
            this.attendeeService = unitOfWork.AttendeeService;
            this.groupService = unitOfWork.GroupService;
            this.mapper = AutoMapperConfiguration.GetInstance();
        }

        public AttendeeNetworkService(UdpClient localServer, IPEndPoint remoteHostEP)
        {
            this.localServer = localServer;
            this.remoteHostEP = remoteHostEP;
        }

        public async Task<AttendeeViewModel> Start(NetworkMessageViewModel message)
        {
            if (localServer == null) localServer = new UdpClient();

            string reqMess = JsonConvert.SerializeObject(message);
            var remoteHostEP = new IPEndPoint(IPAddress.Broadcast, NetworkUtils.RunningPort);
            if (this.remoteHostEP != null) remoteHostEP = this.remoteHostEP;

            Communicator communicator = new Communicator(localServer, ref remoteHostEP);
            communicator.Send(Encoding.UTF8.GetBytes(reqMess));

            object responseData = communicator.Receive();
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

    }
}
