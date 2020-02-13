using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AttendanceSystemIPCamera.Services.GroupService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRecordService : IBaseService<Record>
    {
        List<RecordAttendanceViewModel> GetRecord(RecordSearchViewModel searchViewModel);
    }
    public class RecordService : BaseService<Record>, IRecordService
    {
        private IRecordRepository recordRepository;

        private IAttendeeService attendeeService;

        public RecordService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.recordRepository = unitOfWork.RecordRepository;
            this.attendeeService = unitOfWork.AttendeeService;
        }

        public List<RecordAttendanceViewModel> GetRecord(RecordSearchViewModel searchViewModel)
        {
            var attendee = attendeeService.GetById(searchViewModel.AttendeeId);
            if(attendee != null)
            {
                List<Record> records = recordRepository.GetByRecordSearch(searchViewModel);
                var recordAttendanceViewModel = records.Select(r => new RecordAttendanceViewModel()
                {
                    Id = r.Id,
                    StartTime = r.Session.StartTime,
                    Duration = r.Session.Duration,
                    GroupCode = r.Session.Group.Code
                }).ToList();
                return recordAttendanceViewModel;
            }
            throw new BaseException(ErrorMessage.ATTENDEE_NOT_FOUND);
        }

    }
}
