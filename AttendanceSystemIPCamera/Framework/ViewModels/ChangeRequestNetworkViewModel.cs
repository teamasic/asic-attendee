﻿using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ChangeRequestNetworkViewModel
    {
        public int RecordId { get; set; }
        public string Comment { get; set; }
        public DateTime DateSubmitted { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
    public class ChangeRequestViewModel
    {
        public int RecordId { get; set; }
        public string Comment { get; set; }
        public DateTime DateSubmitted { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
    public class ChangeRequestExpandedViewModel
    {
        public int ExternalRecordId { get; set; }
        public int LocalRecordId { get; set; }
        public string AttendeeCode { get; set; }
        public DateTime RecordStartTime { get; set; }
        public string Comment { get; set; }
        public DateTime DateSubmitted { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}
