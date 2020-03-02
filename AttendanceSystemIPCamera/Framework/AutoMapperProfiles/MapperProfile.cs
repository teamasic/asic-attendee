using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AutoMapperProfiles
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Group, GroupViewModel>().ReverseMap();
            CreateMap<GroupNetworkViewModel, GroupViewModel>().ReverseMap();

            CreateMap<Session, SessionViewModel>().ReverseMap();
            CreateMap<Session, SessionAttendanceViewModel>().ReverseMap();

            CreateMap<Attendee, AttendeeViewModel>().ReverseMap();

            CreateMap<Record, RecordViewModel>().ReverseMap();
            CreateMap<RecordNetworkViewModel, RecordViewModel>().ReverseMap();
            CreateMap<Record, RecordSimpleViewModel>().ReverseMap();

            CreateMap<ChangeRequest, ChangeRequestSimpleViewModel>().ReverseMap();
            CreateMap<ChangeRequest, CreateChangeRequestViewModel>().ReverseMap();
            CreateMap<ChangeRequest, ProcessChangeRequestViewModel>().ReverseMap();
        }
    }
}
