﻿using AttendanceSystemIPCamera.Models;
using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class BaseViewModel<TEntity> where TEntity : class, new()
    {
        public TEntity ToEntity()
        {
            return AutoMapperConfiguration.GetInstance().Map<TEntity>(this);
        }
    }
}
