using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.AttendeeService;
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
        List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, string groupCode);
        Task<List<Session>> AddSessionsIfNotInDbAsync(List<SessionViewModel> sessionVms);
    }

    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;

        private IMapper mapper;

        public SessionService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;

            this.mapper = unitOfWork.mapper;
        }

        public async Task<List<Session>> AddSessionsIfNotInDbAsync(List<SessionViewModel> sessionVms)
        {
            var sessionsReturn = new List<Session>();
            var groupCodes = sessionVms.Select(s => s.GroupCode).Distinct().ToList();
            foreach (var gCode in groupCodes)
            {
                var sessionInGroupVms = sessionVms.Where(svm => svm.GroupCode == gCode).ToList();

                //find session not exist
                var startTimes = sessionVms.Select(ss => ss.StartTime).ToList();
                var sessionInDb = sessionRepository.GetByStartTimesAndGroupCode(startTimes, gCode);
                var startTimesInDb = sessionInDb.Select(s => s.StartTime).ToList();
                var startTimesNotInDb = startTimes.Where(st => !startTimesInDb.Contains(st)).ToList();
                var sessionVmsNotInDb = sessionVms.Where(s => startTimesNotInDb.Contains(s.StartTime)).ToList();

                sessionsReturn.AddRange(sessionInDb);

                //save sessions not exist
                if (sessionVmsNotInDb != null && sessionVmsNotInDb.Count > 0)
                {
                    var sessionsNotInDb = sessionVmsNotInDb.Select(svm => new Session()
                    {
                        Name = svm.Name,
                        StartTime = svm.StartTime,
                        EndTime = svm.EndTime,
                        Status = svm.Status,
                        GroupCode = svm.GroupCode
                    });
                    await sessionRepository.Add(sessionsNotInDb);
                    sessionsReturn.AddRange(sessionsNotInDb);
                }
            }
            unitOfWork.Commit();
            return sessionsReturn;
        }

        public List<SessionViewModel> GetSessionNotInDb(List<SessionViewModel> sessions, string groupCode)
        {
            var startTimes = sessions.Select(ss => ss.StartTime).ToList();
            var sessionInDb = sessionRepository.GetByStartTimesAndGroupCode(startTimes, groupCode);
            var startTimesInDb = sessionInDb.Select(s => s.StartTime).ToList();
            var startTimesNotInDb = startTimes.Where(st => !startTimesInDb.Contains(st)).ToList();
            return sessions.Where(s => startTimesNotInDb.Contains(s.StartTime)).ToList();
        }
    }
}
