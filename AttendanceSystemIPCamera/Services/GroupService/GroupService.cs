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
        Task<List<Group>> AssignAttendeeToGroups(List<GroupViewModel> groupViewModel, string attendeeCode);
    }

    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private IMapper mapper;
        public GroupService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            groupRepository = unitOfWork.GroupRepository;
            mapper = unitOfWork.mapper;
        }

        private async Task<List<Group>> AddGroupIfNotInDbAsync(List<GroupViewModel> groupVMs)
        {
            List<Group> groupsReturn = new List<Group>();

            var groupCodes = groupVMs.Select(gvm => gvm.Code).ToList();
            var groupsInDb = groupRepository.GetByGroupCodesWithAttendeeGroups(groupCodes);
            var groupCodesInDb = groupsInDb.Select(g => g.Code).ToList();
            var groupCodesNotInDb = groupCodes.Where(gc => !groupCodesInDb.Contains(gc)).ToList();
            groupsReturn.AddRange(groupsInDb);

            if (groupCodesNotInDb != null && groupCodesNotInDb.Count > 0)
            {
                var groupsNotInDb = groupCodesNotInDb.Select(gc => new Group()
                {
                    Code = gc,
                    Name = groupVMs.FirstOrDefault(a => a.Code.Equals(gc))?.Name,
                    Deleted = false,
                    DateTimeCreated = DateTime.UtcNow,
                }).ToList();
                await groupRepository.Add(groupsNotInDb);
                unitOfWork.Commit();

                groupsReturn.AddRange(groupsNotInDb);
            }
            return groupsReturn;
        }

        public async Task<List<Group>> AssignAttendeeToGroups(List<GroupViewModel> groupVMs, 
                                                                    string attendeeCode)
        {
            var groups = await AddGroupIfNotInDbAsync(groupVMs);
            groups.ForEach(g =>
            {
                bool isExist = g.AttendeeGroups.Any(ag => ag.AttendeeCode == attendeeCode
                                            && ag.GroupCode == g.Code);
                if (!isExist)
                {
                    g.AttendeeGroups.Add(new AttendeeGroup()
                    {
                        AttendeeCode = attendeeCode,
                        IsActive = true
                    });
                }
            });
            unitOfWork.Commit();
            return groups;
        }
    }
}
