using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class RecordViewModel : BaseViewModel<Record>
    {
        public int AttendeeId { get; set; }
        public bool Present { get; set; }
        public ChangeRequestViewModel ChangeRequest {get; set;}
    }
    public class RecordSimpleViewModel : BaseViewModel<Record>
    {
        public int Id { get; set; }
        public AttendeeViewModel Attendee { get; set; }
        public SessionViewModel Session { get; set; }
        public bool Present { get; set; }
    }

    public class RecordAttendanceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Present { get; set; }
        public ChangeRequestViewModel ChangeRequest { get; set; }
    }

    public class RecordNetworkViewModel : BaseViewModel<Record>
    {
        [JsonIgnore]
        public int AttendeeId { get; set; }
        public bool Present { get; set; }
        public ChangeRequestNetworkViewModel ChangeRequest { get; set; }
    }

}
