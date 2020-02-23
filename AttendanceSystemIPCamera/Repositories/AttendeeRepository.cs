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
    public interface IAttendeeRepository : IRepository<Attendee>
    {
        Attendee GetByCodeWithAttendeeGroups(string code);
        Attendee GetByIdWithAttendeeGroups(int Id);
    }
    public class AttendeeRepository : Repository<Attendee>, IAttendeeRepository
    {
        public AttendeeRepository(DbContext context) : base(context)
        {
        }

        public Attendee GetByCodeWithAttendeeGroups(string code)
        {
            return dbSet.Where(a => code.Equals(a.Code))
                .Include(a => a.AttendeeGroups).FirstOrDefault();
        }
        public Attendee GetByIdWithAttendeeGroups(int Id)
        {
            return dbSet.Where(a => a.Id == Id)
                .Include(a => a.AttendeeGroups).FirstOrDefault();
        }

    }
}
