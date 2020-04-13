using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string GroupCode { get; set; }
        public List<RecordViewModel> Records { get; set; }
        [JsonIgnore]
        public GroupViewModel Group { get; set; }
    }

    public class SessionNetworkViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GroupCode { get; set; }
        public string Status { get; set; }
        public List<RecordNetworkViewModel> Records { get; set; }
    }

    public class SessionSearchViewModel
    {
        [Required]
        public string AttendeeCode { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public List<string> GroupCodes { get; set; }
    }

}
