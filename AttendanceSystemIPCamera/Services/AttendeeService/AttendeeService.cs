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
using AttendanceSystemIPCamera.Services.NetworkService;

namespace AttendanceSystemIPCamera.Services.AttendeeService
{
    public interface IAttendeeService : IBaseService<Attendee>
    {
        Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name, 
                string avatar = "", List<string> groupCodes = null);
        Task<AttendeeViewModel> LoginWithFirebase(UserAuthentication userAuthentication);
    }

    public class AttendeeService : BaseService<Attendee>, IAttendeeService
    {
        private readonly IAttendeeRepository attendeeRepository;
        private readonly AppSettings appSettings;


        private readonly IMapper mapper;

        public AttendeeService(MyUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            this.attendeeRepository = unitOfWork.AttendeeRepository;
            this.appSettings = unitOfWork.AppSettings;

            this.mapper = mapper;
        }

        public async Task<AttendeeViewModel> AddAttendeeWithGroupsIfNotInDb(string attendeeCode, string name, string avatar,
                                                                                List<string> groupCodes = null)
        {
            //add attendee if not exist
            var attendee = attendeeRepository.GetByCodeWithAttendeeGroups(attendeeCode);
            if (attendee == null)
            {
                attendee = new Attendee()
                {
                    Code = attendeeCode,
                    Name = name,
                    Image = avatar
                };
                await this.Add(attendee);
            }

            //add attendee group if not exist
            var attendedGroupCodes = attendee.AttendeeGroups.Select(ag => ag.GroupCode).ToList();
            if (groupCodes != null && groupCodes.Count > 0)
            {
                groupCodes.ForEach(gCode =>
                {
                    if (!attendedGroupCodes.Contains(gCode))
                    {
                        var attGr = new AttendeeGroup()
                        {
                            GroupCode = gCode,
                            AttendeeCode = attendee.Code
                        };
                        attendee.AttendeeGroups.Add(attGr);
                    }
                });
            }
            unitOfWork.Commit();
            return mapper.Map<AttendeeViewModel>(attendee);
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
                (authorizedUser.User.Code, authorizedUser.User.Name, authorizedUser.User.Image);

            //var accessTokenViewModel = new AccessToken()
            //{
            //    Token = authorizedUser.AccessToken,
            //    Attendee = attendee
            //};
            return attendee;

        }
    }
}
