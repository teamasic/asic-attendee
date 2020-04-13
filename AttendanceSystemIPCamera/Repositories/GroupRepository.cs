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
    public interface IGroupRepository : IRepository<Group>
    {
        List<Group> GetByGroupCodesWithAttendeeGroups(List<string> groupCodes);
    }
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(DbContext context) : base(context)
        {
        }
        
        public List<Group> GetByGroupCodesWithAttendeeGroups(List<string> groupCodes)
        {
            return Get(g => groupCodes.Contains(g.Code), includeProperties: "AttendeeGroups").ToList();
        }
    }
}
