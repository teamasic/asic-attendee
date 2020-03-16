using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
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

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        Task SaveAttendanceInfo(AttendanceNetworkViewModel attendanceInfo);
        List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, int groupId);
        Task AddSessionsWithRecordsAsync(List<SessionViewModel> sessionVms, int attendeeId);

        List<SessionAttendanceViewModel> 
            GetAttendanceInfoBySearchVM(SessionSearchViewModel searchViewModel);
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;

        private readonly IGroupService groupService;
        private readonly IAttendeeService attendeeService;

        private IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;

            this.groupService = unitOfWork.GroupService;
            this.attendeeService = unitOfWork.AttendeeService;

            mapper = AutoMapperConfiguration.GetInstance();
        }


        public async Task AddSessionsWithRecordsAsync(List<SessionViewModel> sessionVms, int attendeeId)
        {
            var groupIds = sessionVms.Select(s => s.GroupId).Distinct().ToList();
            foreach (var gId in groupIds)
            {

                var sessionInGroupVms = sessionVms.Where(svm => svm.GroupId == gId);
                //find session not exist
                var startTimes = sessionInGroupVms.Select(ss => ss.StartTime).Distinct().ToList();
                var sessionInDb = sessionRepository.GetByStartTimesAndGroupId(startTimes, gId);
                var startTimesInDb = sessionInDb.Select(s => s.StartTime).ToList();
                var startTimesNotInDb = startTimes.Where(st => !startTimesInDb.Contains(st)).ToList();
                var sessionVmsNotInDb = sessionInGroupVms.Where(s => startTimesNotInDb.Contains(s.StartTime)).ToList();


                //save sessions not exist, sessions contain records
                if (sessionVmsNotInDb != null && sessionVmsNotInDb.Count > 0)
                {
                    var sessionsNotInDb = mapper.ProjectTo<SessionViewModel, Session>(sessionVmsNotInDb);
                    await this.Add(sessionsNotInDb);
                }

                //update record that its session exist
                sessionInDb.ForEach(session =>

                {
                    var s = sessionInGroupVms.FirstOrDefault(s => s.GroupId == session.GroupId
                                        && s.StartTime == session.StartTime);
                    var record = session.Records.LastOrDefault(r => r.AttendeeId == attendeeId);
                    if(record != null)
                    {
                        var networkRecord = s.Records?.LastOrDefault();
                        record.Present = networkRecord?.Present ?? record.Present;
                        if (networkRecord != null && networkRecord.ChangeRequest != null)
                        {
                            record.ChangeRequest.Status = networkRecord.ChangeRequest.Status;
                        }
                    }
                    else
                    {
                        session.Records.Add(new Record()
                        {
                            AttendeeId = attendeeId,
                            Present = s.Records?.LastOrDefault()?.Present ?? false
                        });

                    }
                });
                unitOfWork.Commit();
            }
        }

        public List<SessionAttendanceViewModel> 
            GetAttendanceInfoBySearchVM(SessionSearchViewModel searchViewModel)
        {
            var attendee = attendeeService.GetByIdWithAttendeeGroups(searchViewModel.AttendeeId);
            if (attendee != null)
            {
                var groupIds = attendee.AttendeeGroups.Select(ag => ag.GroupId).ToList();
                searchViewModel.GroupIds = groupIds;
                var sessions = sessionRepository.GetBySearchVM(searchViewModel);
                var recordAttendanceViewModel = mapper.ProjectTo<Session, SessionAttendanceViewModel>(sessions);
                return recordAttendanceViewModel.ToList();
            }
            throw new BaseException(ErrorMessage.ATTENDEE_NOT_FOUND);
        }


        //public async void AddSessionsIfNotInDb(List<SessionViewModel> sessions)
        //{
        //    var groupIds = sessions.Select(s => s.GroupId).ToList();
        //    foreach (var gId in groupIds)
        //    {
        //        var sessionVmsByGroupId = sessions.Where(s => s.GroupId == gId).ToList();
        //        var sessionVmsNotInDb = GetSessionNotInDb(sessionVmsByGroupId, gId);
        //        var sessionsNotInDb = mapper.ProjectTo<SessionViewModel, Session>(sessionVmsNotInDb);
        //        await this.Add(sessionsNotInDb);
        //    }

        //}

        //public async Task SaveAttendanceInfo(AttendanceNetworkViewModel attendanceInfo)
        //{
        //    var tran = unitOfWork.CreateTransaction();
        //    try
        //    {
        //        //--
        //        var attendee = await attendeeService.AddAttendeeWithGroupsIfNotInDb(attendanceInfo.AttendeeCode, attendanceInfo.AttendeeName);

        //        var groupsWithSessions = attendanceInfo.Groups;
        //        var groups = groupsWithSessions.Select(a => new GroupViewModel()
        //        {
        //            Code = a.Code,
        //            Name = a.Name,
        //        }).Distinct().ToList();
        //        await groupService.AddGroupIfNotInDbAsync(groups);
        //        //--
        //        //add attendee groups

        //        foreach (var group in groupsWithSessions)
        //        {
        //            var currentGroup = await groupRepository.GetByGroupCodeAsync(group.Code);
        //            var sessionsNotInDb = GetSessionNotInDb(group.Sessions, currentGroup.Id);
        //            if (sessionsNotInDb != null && sessionsNotInDb.Count > 0)
        //            {
        //                var sessions = sessionsNotInDb.Select(s =>
        //                {
        //                    var session = s.ToEntity();
        //                    session.Id = 0;
        //                    session.GroupId = currentGroup.Id;
        //                    //if (s.Record != null)
        //                    //{
        //                    //    session.Records.Add(new Record()
        //                    //    {
        //                    //        AttendeeId = attendee.Id,
        //                    //        Present = s.Record.Present
        //                    //    });
        //                    //}
        //                    return session;
        //                }).ToList();
        //                await sessionRepository.Add(sessions);
        //                unitOfWork.Commit();
        //            }

        //        }
        //        tran.Commit();
        //    }
        //    catch (Exception)
        //    {
        //        tran.Rollback();
        //        throw;
        //    }
        //}

        public List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, int groupId)
        {
            var startTimes = sessions.Select(ss => ss.StartTime).ToList();
            var sessionInDb = sessionRepository.GetByStartTimesAndGroupId(startTimes, groupId);
            var startTimesInDb = sessionInDb.Select(s => s.StartTime).ToList();
            var startTimesNotInDb = startTimes.Where(st => !startTimesInDb.Contains(st)).ToList();
            return sessions.Where(s => startTimesNotInDb.Contains(s.StartTime)).ToList();
        }



        public Task SaveAttendanceInfo(AttendanceNetworkViewModel attendanceInfo)
        {
            throw new NotImplementedException();
        }
    }
}
