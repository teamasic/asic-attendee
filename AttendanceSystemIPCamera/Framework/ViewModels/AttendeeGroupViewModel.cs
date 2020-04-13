using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class AttendeeGroupViewModel
    {
        public int Id { get; set; }
        public string AttendeeCode { get; set; }
        public string GroupCode { get; set; }
        public bool IsActive { get; set; }
    }
}
