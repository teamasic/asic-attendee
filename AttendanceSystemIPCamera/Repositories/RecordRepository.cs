using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IRecordRepository : IRepository<Record>
    {
        List<Record> GetByRecordSearch(RecordSearchViewModel search);

    }
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(DbContext context) : base(context)
        {
        }

        public List<Record> GetByRecordSearch(RecordSearchViewModel search)
        {
            var records = dbSet.Where(r => r.AttendeeId == search.AttendeeId)
                .Where(r => r.Session.StartTime > search.StartTime)
                .Where(r => r.Session.StartTime <= search.EndTime)
                .Include(r => r.Session.Group)
                .ToList();
            return records;
        }
    }
}
