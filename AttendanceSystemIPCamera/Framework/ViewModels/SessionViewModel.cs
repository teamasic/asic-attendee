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
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        public int GroupId { get; set; }
        public List<RecordViewModel> Records { get; set; }
        [JsonIgnore]
        public GroupViewModel Group { get; set; }
    }

    public class SessionNetworkViewModel : BaseViewModel<Session>
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int GroupId { get; set; }
        public List<RecordNetworkViewModel> Records { get; set; }
    }

    public class SessionAttendanceViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool? Present => Records?.LastOrDefault()?.Present;
        public string GroupCode => Group.Code;
        [JsonIgnore]
        public Group Group { get; set; }
        [JsonIgnore]
        public virtual ICollection<Record> Records { get; set; }

    }

    public class SessionSearchViewModel
    {
        [Required]
        public int AttendeeId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        public List<int> GroupIds { get; set; }
    }

}
