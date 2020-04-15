using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using AttendanceSystemIPCamera.Framework.ViewModels;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IChangeRequestRepository : IRepository<ChangeRequest>
    {
        public Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel);
        public Task<ChangeRequest> GetByIdSimple(object id);
        List<ChangeRequest> GetByRecordIds(List<int> recordIds);
    }
    public class ChangeRequestRepository : Repository<ChangeRequest>, IChangeRequestRepository
    {
        public ChangeRequestRepository(DbContext context) : base(context)
        {
        }

        public async Task<ChangeRequest> GetByIdSimple(object id)
        {
            return await dbSet
                .Include(cr => cr.Record)
                .FirstOrDefaultAsync(cr => cr.RecordId == (int)id);
        }

        public new async Task<ChangeRequest> GetById(object id)
        {
            return await dbSet
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.AttendeeGroup.Attendee)
                .Include(cr => cr.Record)
                   .ThenInclude(r => r.Session)
                        .ThenInclude(s => s.Group)
                .FirstOrDefaultAsync(cr => cr.RecordId == (int)id);
        }
        public async Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel)
        {
            bool filterStatus(ChangeRequest cr)
            {
                switch (viewModel.Status)
                {
                    case ChangeRequestStatusFilter.UNRESOLVED:
                        return !cr.IsResolved;
                    case ChangeRequestStatusFilter.RESOLVED:
                        return cr.IsResolved;
                    case ChangeRequestStatusFilter.ALL:
                        return true;
                    default:
                        return true;
                }
            }
            return dbSet
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.AttendeeGroup.Attendee)
                .Include(cr => cr.Record)
                    .ThenInclude(r => r.Session)
                        .ThenInclude(s => s.Group)
                .Where(filterStatus);
        }

        public List<ChangeRequest> GetByRecordIds(List<int> recordIds)
        {
            return Get(c => recordIds.Contains(c.RecordId)).ToList();
        }
    }
}
