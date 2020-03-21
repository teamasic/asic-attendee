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
using AttendanceSystemIPCamera.Framework.MyConfiguration;

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name, List<int> groupIds);
        Task<AttendeeViewModel> Login(LoginViewModel loginViewModel);
        Attendee GetByIdWithAttendeeGroups(int attendeeId);
        Task<AttendeeViewModel> LoginWithFirebase(UserAuthentication userAuthentication);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        private readonly AppSettings appSettings;

        public AttendeeService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.attendeeRepository = unitOfWork.AttendeeRepository;
            this.appSettings = unitOfWork.AppSettings;
        }

        public async Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name,
                                                                                List<int> groupIds = null)
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
            if (groupIds != null && groupIds.Count > 0)
            {
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
            }
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
            //IPAddress localIp = null;
            //IPAddress.TryParse(NetworkUtils.GetLocalIPAddress(), out localIp);
            //if (localIp != null)
            //{
            //    var networkRequest = new NetworkRequest<LoginViewModel>()
            //    {
            //        IPAddress = localIp.ToString(),
            //        Route = NetworkRoute.LOGIN,
            //        Request = loginViewModel
            //    };
            //    var attendee = await unitOfWork.AttendeeNetworkService.Start(networkRequest);
            //    if (attendee != null) return attendee;
            //    throw new BaseException(ErrorMessage.LOGIN_FAIL);
            //}
            //else throw new BaseException(ErrorMessage.CANNOT_GET_LOCAL_IP_ADDRESS);

            return await unitOfWork.AttendeeNetworkService.Login(loginViewModel);
        }

        public Attendee GetByIdWithAttendeeGroups(int attendeeId)
        {
            return attendeeRepository.GetByIdWithAttendeeGroups(attendeeId);
        }

        public async Task<AttendeeViewModel> LoginWithFirebase(UserAuthentication userAuthentication)
        {
            var authorizedUser = await RestApi.CallApiAsync<AuthorizedUser>(appSettings.LoginServerApi, userAuthentication);
            if (authorizedUser == null)
            {
                throw new BaseException(ErrorMessage.ATTENDEE_NOT_FOUND);
            }

            //check role
            var attendeeRole = (int)RolesEnum.ATTENDEE;
            if (!authorizedUser.Roles.Contains(attendeeRole.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                throw new BaseException(ErrorMessage.NOT_VALID_USER);
            }

            //check attendee in local app
            var attendee =
                await this.AddAttendeeWithGroupsIfNotInDb
                (authorizedUser.User.RollNumber, authorizedUser.User.Fullname);

            //var accessTokenViewModel = new AccessToken()
            //{
            //    Token = authorizedUser.AccessToken,
            //    Attendee = attendee
            //};
            return attendee;

        }
    }
}
