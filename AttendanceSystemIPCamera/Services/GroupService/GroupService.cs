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

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IGroupService : IBaseService<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        Task AddGroupIfNotInDb(List<GroupViewModel> groupViewModel);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISessionRepository sessionRepository;
        public GroupService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            sessionRepository = unitOfWork.SessionRepository;
        }

        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            return await groupRepository.GetAll(groupSearchViewModel);
        }

        public async Task AddGroupIfNotInDb(List<GroupViewModel> groupViewModel)
        {
            var groupCodes = groupViewModel.Select(gvm => gvm.Code).ToList();
            var groupsInDb = groupRepository.GetByGroupCodes(groupCodes);
            var groupCodesInDb = groupsInDb.Select(g => g.Code).ToList();
            var groupCodesNotInDb = groupCodes.Where(gc => !groupCodesInDb.Contains(gc)).ToList();
            if (groupCodesNotInDb != null && groupCodesNotInDb.Count > 0)
            {
                var groupsNotInDb = groupCodesNotInDb.Select(gc => new Group()
                {
                    Code = gc,
                    Name = groupViewModel.FirstOrDefault(a => a.Code.Equals(gc))?.Name,
                    Deleted = false,
                    DateTimeCreated = DateTime.UtcNow
                });
                await groupRepository.Add(groupsNotInDb);
                unitOfWork.Commit();
            }
        }
    }
}
