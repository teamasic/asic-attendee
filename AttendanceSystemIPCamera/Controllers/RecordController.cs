﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.RecordService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : BaseController
    {
        private readonly IRecordService service;
        public RecordController(IRecordService service)
        {
            this.service = service;
        }

        [HttpGet]
        public dynamic GetRecords([FromQuery] SessionSearchViewModel search)
        {
            return ExecuteInMonitoring(() =>
            {
                return service.GetRecord(search);
            });
        }

        [HttpGet("refresh")]
        public async Task<dynamic> Refresh([FromQuery] SessionSearchViewModel searchViewModel)
        {
            return await ExecuteInMonitoring( async () =>
            {
                return await service.Refresh(searchViewModel);
            });
        }

    }
}
