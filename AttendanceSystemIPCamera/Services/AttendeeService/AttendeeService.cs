using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;
using System.Net;
using AttendanceSystemIPCamera.Utils;
using AttendanceSystemIPCamera.Framework;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name, List<int> groupIds);
        Task<AttendeeViewModel> Login(LoginViewModel loginViewModel);
        Attendee GetByIdWithAttendeeGroups(int attendeeId);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;

        public AttendeeService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.attendeeRepository = unitOfWork.AttendeeRepository;
        }

        public async Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name,
                                                                                List<int> groupIds)
        {
            var attendee = attendeeRepository.GetByCodeWithAttendeeGroups(attendeeCode);
            if (attendee == null)
            {
                attendee = new Attendee()
                {
                    Code = attendeeCode,
                    Name = name,
                };
                await this.Add(attendee);
            }

            var attendedGroupIds = attendee.AttendeeGroups.Select(ag => ag.GroupId).ToList();
            groupIds.ForEach(gId =>
            {
                if (!attendedGroupIds.Contains(gId))
                {
                    var attGr = new AttendeeGroup()
                    {
                        AttendeeId = attendee.Id,
                        GroupId = gId
                    };
                    attendee.AttendeeGroups.Add(attGr);
                }
            });
            unitOfWork.Commit();
            return new AttendeeViewModel()
            {
                Id = attendee.Id,
                Code = attendee.Code,
                Name = attendee.Name
            };
        }

        public async Task<AttendeeViewModel> Login(LoginViewModel loginViewModel)
        {
            IPAddress localIp = null;
            IPAddress.TryParse(NetworkUtils.GetLocalIPAddress(), out localIp);
            if (localIp != null)
            {
                var networkMessage = new NetworkMessageViewModel()
                {
                    IPAddress = localIp.ToString(),
                    Message = loginViewModel
                };
                var attendee = await unitOfWork.AttendeeNetworkService.Start(networkMessage);
                if (attendee != null) return attendee;
                throw new BaseException(ErrorMessage.LOGIN_FAIL);
            }
            else throw new BaseException(ErrorMessage.CANNOT_GET_LOCAL_IP_ADDRESS);
        }

        public Attendee GetByIdWithAttendeeGroups(int attendeeId)
        {
            return attendeeRepository.GetByIdWithAttendeeGroups(attendeeId);
        }
    }
}
