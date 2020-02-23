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
        Task<Group> GetByGroupCodeAsync(string code);
        List<Group> GetByGroupCodes(List<string> groupCodes);
        Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        List<string> GetGroupCodesContainsInList(List<string> groupCodes);


    }
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(DbContext context) : base(context)
        {
        }
        private Func<IQueryable<Group>, IOrderedQueryable<Group>> Order(OrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderBy.Name:
                    return groups => groups.OrderBy(g => g.Name);
                case OrderBy.DateCreated:
                default:
                    return groups => groups.OrderByDescending(g => g.DateTimeCreated);
            }
        }
        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            var orderFunction = Order(groupSearchViewModel.OrderBy);
            var query = dbSet
                .Include(g => g.AttendeeGroups)
                    .ThenInclude(ag => ag.Attendee)
                .Include(g => g.Sessions)
                .Where(g => g.Name.ToLower().Contains(groupSearchViewModel.NameContains.ToLower()));
            query = orderFunction(query);
            return await PaginatedList<Group>.CreateAsync(query, groupSearchViewModel.Page, groupSearchViewModel.PageSize);
        }

        public async Task<Group> GetByGroupCodeAsync(string code)
        {
            return await dbSet.FirstOrDefaultAsync(g => g.Code.Equals(code));
        }

        public List<Group> GetByGroupCodes(List<string> groupCodes)
        {
            return dbSet.Where(g => groupCodes.Contains(g.Code)).ToList();
        }

        public List<string> GetGroupCodesContainsInList(List<string> groupCodes)
        {
            return this.dbSet.Where(g => groupCodes.Contains(g.Code))
                            .Select(g => g.Code)
                            .ToList();
        }
    }
}
