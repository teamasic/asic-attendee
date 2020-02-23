using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.SessionService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : BaseController
    {
        private readonly ISessionService service;
        public SessionController(ISessionService service)
        {
            this.service = service;
        }

        [HttpGet]
        public dynamic GetAttendanceInfo([FromQuery] SessionSearchViewModel searchVM)
        {
            return ExecuteInMonitoring(() =>
         {
             return service.GetAttendanceInfoBySearchVM(searchVM);
         });
        }


    }
}
