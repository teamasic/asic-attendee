using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class GroupViewModel : BaseViewModel<Group>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Session> Sessions { get; set; }

    }

    public class GroupNetworkViewModel : BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SessionNetworkViewModel> Sessions { get; set; }

    }
}
