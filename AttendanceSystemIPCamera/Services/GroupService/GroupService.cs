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
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework;

namespace AttendanceSystemIPCamera.Services.GroupService
{
    public interface IGroupService : IBaseService<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        Task<List<GroupViewModel>> AddGroupIfNotInDbAsync(List<GroupViewModel> groupViewModel);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISessionRepository sessionRepository;
        private IMapper mapper;
        public GroupService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            sessionRepository = unitOfWork.SessionRepository;
            mapper = AutoMapperConfiguration.GetInstance();
        }

        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            return await groupRepository.GetAll(groupSearchViewModel);
        }

        public async Task<List<GroupViewModel>> AddGroupIfNotInDbAsync(List<GroupViewModel> groupViewModel)
        {
            List<Group> groupsReturn = new List<Group>();

            var groupCodes = groupViewModel.Select(gvm => gvm.Code).ToList();
            var groupsInDb = groupRepository.GetByGroupCodes(groupCodes);
            var groupCodesInDb = groupsInDb.Select(g => g.Code).ToList();
            //var groupCodesInDb = groupRepository.GetGroupCodesContainsInList(groupCodes);
            var groupCodesNotInDb = groupCodes.Where(gc => !groupCodesInDb.Contains(gc)).ToList();
            groupsReturn.AddRange(groupsInDb);

            if (groupCodesNotInDb != null && groupCodesNotInDb.Count > 0)
            {
                var groupsNotInDb = groupCodesNotInDb.Select(gc => new Group()
                {
                    Code = gc,
                    Name = groupViewModel.FirstOrDefault(a => a.Code.Equals(gc))?.Name,
                    Deleted = false,
                    DateTimeCreated = DateTime.UtcNow,
                }).ToList();
                await groupRepository.Add(groupsNotInDb);
                unitOfWork.Commit();

                groupsReturn.AddRange(groupsNotInDb);
            }
            return mapper.ProjectTo<Group, GroupViewModel>(groupsReturn).ToList();
        }
    }
}
