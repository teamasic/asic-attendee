using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Services.GroupService;

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        Task SaveAttendanceInfo(AttendanceViewModel attendanceInfo);
        List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, int groupId);

    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;

        private readonly IGroupService groupService;
        private readonly IAttendeeService attendeeService;

        public SessionService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;

            this.groupService = unitOfWork.GroupService;
            this.attendeeService = unitOfWork.AttendeeService;
        }

        public async Task SaveAttendanceInfo(AttendanceViewModel attendanceInfo)
        {
            var tran = unitOfWork.CreateTransaction();
            try
            {
                //--
                var attendee = await attendeeService.AddAttendeeIfNotInDb(attendanceInfo.AttendeeCode, attendanceInfo.AttendeeName);

                var groupsWithSessions = attendanceInfo.Groups;
                var groups = groupsWithSessions.Select(a => new GroupViewModel()
                {
                    Code = a.GroupCode,
                    Name = a.Name,
                }).Distinct().ToList();
                await groupService.AddGroupIfNotInDb(groups);
                //--
                //add attendee groups

                foreach (var group in groupsWithSessions)
                {
                    var currentGroup = await groupRepository.GetByGroupCodeAsync(group.GroupCode);
                    var sessionsNotInDb = GetSessionNotInDb(group.Sessions, currentGroup.Id);
                    if (sessionsNotInDb != null && sessionsNotInDb.Count > 0)
                    {
                        var sessions = sessionsNotInDb.Select(s =>
                        {
                            var session = s.ToEntity();
                            session.Id = 0;
                            session.GroupId = currentGroup.Id;
                            if (s.Record != null)
                            {
                                session.Records.Add(new Record()
                                {
                                    AttendeeId = attendee.Id,
                                    Present = s.Record.Present
                                });
                            }
                            return session;
                        }).ToList();
                        await sessionRepository.Add(sessions);
                        unitOfWork.Commit();
                    }
                }
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, int groupId)
        {
            var startTimes = sessions.Select(ss => ss.StartTime).ToList();
            var sessionInDb = sessionRepository.GetByStartTimeAndGroupId(startTimes, groupId);
            var startTimesInDb = sessionInDb.Select(s => s.StartTime).ToList();
            var startTimesNotInDb = startTimes.Where(st => !startTimesInDb.Contains(st)).ToList();
            return sessions.Where(s => startTimesNotInDb.Contains(s.StartTime)).ToList();
        }

    }
}
