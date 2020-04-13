﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class NetworkRequest<T> where T : class
    {
        public string Route { get; set; }
        public string IPAddress { get; set; }
        public T Request { get; set; }
    }
}
