using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AttendanceSystemIPCamera.Framework
{
    class AutoMapperConfiguration
    {
        private static IMapper mapper = null;
        public static IMapper GetInstance()
        {
            if (mapper == null)
            {
                mapper = Mapper.Configuration.CreateMapper();
            }
            return mapper;
        }
    }
}
