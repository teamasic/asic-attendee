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
        List<RecordAttendanceViewModel> GetRecord(SessionSearchViewModel searchViewModel);
        Task<List<RecordAttendanceViewModel>> Refresh(SessionSearchViewModel searchViewModel);
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

        public List<RecordAttendanceViewModel> GetRecord(SessionSearchViewModel searchViewModel)
        {
            var attendee = attendeeService.GetById(searchViewModel.AttendeeId);
            if (attendee != null)
            {
                List<Record> records = recordRepository.GetByRecordSearch(searchViewModel);
                var recordAttendanceViewModel = records.Select(r => {
                    var rec = new RecordAttendanceViewModel()
                    {
                        Id = r.Id,
                        Name = r.Session.Name,
                        StartTime = r.Session.StartTime,
                        EndTime = r.Session.EndTime,
                        GroupCode = r.Session.Group.Code,
                        GroupName = r.Session.Group.Name,
                        Present = r.Present
                    };
                    if (r.ChangeRequest != null)
                    {
                        rec.ChangeRequest = new ChangeRequestViewModel
                        {
                            Id = r.ChangeRequest.Id,
                            Comment = r.ChangeRequest.Comment,
                            RecordId = r.Id,
                            Status = r.ChangeRequest.Status
                        };
                    }
                    return rec;
                })
                .ToList();
                return recordAttendanceViewModel;
            }
            throw new BaseException(ErrorMessage.ATTENDEE_NOT_FOUND);
        }


        public async Task<List<RecordAttendanceViewModel>> Refresh(SessionSearchViewModel searchViewModel)
        {
            var attendee = await attendeeService.GetById(searchViewModel.AttendeeId);
            await attendeeService.Login(new LoginViewModel()
            {
                AttendeeCode = attendee.Code,
                LoginMethod = Constant.GET_DATA_BY_ATTENDEE_CODE
            });
            return GetRecord(searchViewModel);
        }

    }
}
