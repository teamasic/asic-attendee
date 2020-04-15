﻿using AttendanceSystemIPCamera.Framework.ViewModels;
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
        List<Record> GetByRecordSearch(SessionSearchViewModel search);

        List<Record> GetByAttendeeGroupIdAndSessionIds(int attendeeGroupId, List<int> sessionIds);

    }
    public class RecordRepository : Repository<Record>, IRecordRepository
    {
        public RecordRepository(DbContext context) : base(context)
        {
        }

        public new async Task<Record> GetById(object id)
        {
            return await dbSet
                .Include(r => r.Session)
                .FirstOrDefaultAsync(r => r.Id == (int) id);
        }

        public List<Record> GetByRecordSearch(SessionSearchViewModel search)
        {
            var records = dbSet.Where(r => r.AttendeeCode == search.AttendeeCode)
                .Where(r => r.Session.StartTime > search.StartTime)
                .Where(r => r.Session.StartTime <= search.EndTime)
                .Include(r => r.Session.Group)
                .Include(r => r.ChangeRequest)
                .ToList();
            return records;
        }

        public List<Record> GetByAttendeeGroupIdAndSessionIds(int attendeeGroupId, List<int> sessionIds)
        {
            return Get(r => r.AttendeeGroupId == attendeeGroupId && sessionIds.Contains(r.SessionId)).ToList();
        }
    }
}
