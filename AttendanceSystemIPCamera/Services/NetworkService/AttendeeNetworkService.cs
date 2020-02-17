using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        private IAttendeeRepository attendeeRepository;

        public AttendeeNetworkService(MyUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.sessionService = unitOfWork.SessionService;
            this.attendeeRepository = unitOfWork.AttendeeRepository;
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
            var attendanceInfo = JsonConvert.DeserializeObject<AttendanceViewModel>(responseData.ToString());
            if(attendanceInfo.Success)
            {
                await sessionService.SaveAttendanceInfo(attendanceInfo);
                var attendee = attendeeRepository.GetByCode(attendanceInfo.AttendeeCode);
                return new AttendeeViewModel()
                {
                    Id = attendee.Id,
                    Code = attendee.Code,
                    Name = attendee.Name
                };
            }
            return null;
        }
    }
}
