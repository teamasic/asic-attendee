using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface ISessionRepository: IRepository<Session>
    {
        List<Session> GetByStartTimesAndGroupId(List<DateTime> startTimes, int groupId);
        List<Session> GetBySearchVM(SessionSearchViewModel searchVM);

    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(DbContext context) : base(context)
        {
        }

        public List<Session> GetBySearchVM(SessionSearchViewModel search)
        {
            var sessions = dbSet.Where(r => r.StartTime > search.StartTime
                                                                && r.EndTime < search.EndTime)
                .Where(s => s.Records.Any(r => r.AttendeeId == search.AttendeeId));
            if(search.GroupIds != null && search.GroupIds.Count > 0)
            {
                
                sessions = sessions.Where(s => search.GroupIds.Contains(s.GroupId))
                    .Include(s=>s.Group);
            }
            return sessions.ToList();
        }

        public List<Session> GetByStartTimesAndGroupId(List<DateTime> startTimes, int groupId)
        {
            return dbSet.Where(s => s.Group.Id == groupId)
                .Where(s => startTimes.Contains(s.StartTime))
                .Include(s => s.Records)
                .ToList();
        }
    }
}
