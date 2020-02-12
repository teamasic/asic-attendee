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
        List<Session> GetByStartTimeAndGroupId(List<DateTime> startTimes, int groupId);

    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(DbContext context) : base(context)
        {
        }

        public List<Session> GetByStartTimeAndGroupId(List<DateTime> startTimes, int groupId)
        {
            return dbSet.Where(s => s.Group.Id == groupId)
                .Where(s => startTimes.Contains(s.StartTime))
                .ToList();
        }
    }
}
