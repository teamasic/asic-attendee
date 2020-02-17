using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AttendanceSystemIPCamera.Framework
{
    class AutoMapperConfiguration
    {
        public static IMapper GetInstance()
        {
            return Mapper.Configuration.CreateMapper();
        }
    }
}
