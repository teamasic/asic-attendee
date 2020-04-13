using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.GroupService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeeController : BaseController
    {
        private readonly IAttendeeService service;
        public AttendeeController(IAttendeeService service)
        {
            this.service = service;
        }


        [HttpPost("login/firebase")]
        public async Task<dynamic> LoginWithFirebase(UserAuthentication userAuthen)
        {
            return await ExecuteInMonitoring(async () =>
            {
                return await service.LoginWithFirebase(userAuthen);
            });
        }


    }
}
