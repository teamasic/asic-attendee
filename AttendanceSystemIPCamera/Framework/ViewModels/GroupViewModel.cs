using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class GroupViewModel : BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TotalSession { get; set; }
        public bool Deleted { get; set; } = false;
        public List<Session> Sessions { get; set; }

    }

    public class GroupNetworkViewModel : BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TotalSession { get; set; }
        public List<SessionNetworkViewModel> Sessions { get; set; }

    }
}
