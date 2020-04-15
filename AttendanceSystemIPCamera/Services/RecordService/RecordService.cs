using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AttendanceSystemIPCamera.Services.GroupService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.NetworkService;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRecordService : IBaseService<Record>
    {
        List<RecordAttendanceViewModel> GetRecord(SessionSearchViewModel searchViewModel);
        Task<List<RecordAttendanceViewModel>> Refresh(SessionSearchViewModel searchViewModel);
        Task<List<Record>> AddOrUpdateRecords(List<RecordViewModel> recordVms);
    }
    public class RecordService : BaseService<Record>, IRecordService
    {
        private IRecordRepository recordRepository;
        private IAttendeeRepository attendeeRepository;

        //private IAttendeeNetworkService attendeeNetworkService;
        private IMapper mapper;


        public RecordService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.recordRepository = unitOfWork.RecordRepository;
            this.attendeeRepository = unitOfWork.AttendeeRepository;

            //this.attendeeNetworkService = unitOfWork.AttendeeNetworkService;

            this.mapper = unitOfWork.mapper;
        }

        public List<RecordAttendanceViewModel> GetRecord(SessionSearchViewModel searchViewModel)
        {
            var attendee = attendeeRepository.GetById(searchViewModel.AttendeeCode);
            if (attendee != null)
            {
                List<Record> records = recordRepository.GetByRecordSearch(searchViewModel);
                var recordAttendanceViewModel = records.Select(r =>
                {
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
                            Comment = r.ChangeRequest.Comment,
                            RecordId = r.Id,
                            Status = r.ChangeRequest.Status,
                            DateSubmitted = r.ChangeRequest.DateSubmitted
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
            var attendee = await attendeeRepository.GetById(searchViewModel.AttendeeCode);
            await unitOfWork.AttendeeNetworkService.Refresh(new LoginViewModel()
            {
                AttendeeCode = attendee.Code,
                LoginMethod = Constant.GET_DATA_BY_ATTENDEE_CODE
            });
            return GetRecord(searchViewModel);
        }

        public async Task<List<Record>> AddOrUpdateRecords(List<RecordViewModel> recordVms)
        {
            var recordsReturn = new List<Record>();
            //This attendeeGroupId is in local database since we assigned to
            var attendeeGroupIds = recordVms.Select(r => r.AttendeeGroupId).ToHashSet();
            foreach (var attendeeGroupId in attendeeGroupIds)
            {
                //This sessionIds is in local database since we assigned to
                var sessionIds = recordVms.Where(r => r.AttendeeGroupId == attendeeGroupId)
                                            .Select(r => r.SessionId)
                                            .ToList();
                var recordsInDb = recordRepository.GetByAttendeeGroupIdAndSessionIds(attendeeGroupId, sessionIds);
                var sessionIdsInDb = recordsInDb.Select(r => r.SessionId).ToList();
                var recordsNotInDb = recordVms.Where(r => r.AttendeeGroupId == attendeeGroupId)
                                              .Where(r => !sessionIdsInDb.Contains(r.SessionId))
                                              .ToList();

                //add not exist records
                if (recordsNotInDb != null && recordsNotInDb.Count > 0)
                {
                    var records = mapper.ProjectTo<RecordViewModel, Record>(recordsNotInDb);
                    await recordRepository.Add(records);
                    recordsReturn.AddRange(records);
                }

                //update exist records
                if (recordsInDb != null && recordsInDb.Count > 0)
                {
                    recordsInDb.ForEach(recordInDb =>
                    {
                        var recordVm = recordVms.Where(r => r.AttendeeGroupId == attendeeGroupId)
                                                .Where(r => r.SessionId == recordInDb.SessionId)
                                                .First();
                        recordInDb.Present = recordVm.Present;
                        recordInDb.UpdateTime = recordVm.UpdateTime;
                    });
                    recordsReturn.AddRange(recordsInDb);
                }
            }
            unitOfWork.Commit();
            return recordsReturn;
        }

    }
}
