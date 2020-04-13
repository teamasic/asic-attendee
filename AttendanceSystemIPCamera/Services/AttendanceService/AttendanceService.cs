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
using System.Net;
using AttendanceSystemIPCamera.Utils;
using AttendanceSystemIPCamera.Framework;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Framework.MyConfiguration;
using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using AttendanceSystemIPCamera.Services.GroupService;

namespace AttendanceSystemIPCamera.Services.AttendanceService
{
    public interface IAttendanceService
    {
        Task<AttendeeViewModel> SaveAttendanceDataAsync(AttendanceNetworkViewModel attendanceInfo);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly IMapper mapper;
        private ISessionService sessionService;
        private IRecordService recordService;
        private IChangeRequestService changeRequestService;
        private IGroupService groupService;
        private MyUnitOfWork unitOfWork;

        public AttendanceService(MyUnitOfWork unitOfWork, IMapper mapper, ISessionService sessionService,
                                        IGroupService groupService,
                                        IRecordService recordService, IChangeRequestService changeRequestService)
        {
            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

            this.sessionService = sessionService;
            this.groupService = groupService;
            this.recordService = recordService;
            this.changeRequestService = changeRequestService;
        }

        public async Task<AttendeeViewModel> SaveAttendanceDataAsync(AttendanceNetworkViewModel attendanceInfo)
        {
            AttendeeViewModel attendee = null;
            using (var scope = unitOfWork.CreateTransaction())
            {
                try
                {
                    string attendeeCode = attendanceInfo.AttendeeCode;
                    //assign attendee to groups
                    var groupVMs = attendanceInfo.Groups.Select(g => new GroupViewModel()
                    {
                        Code = g.Code,
                        Name = g.Name,
                        DateTimeCreated = g.DateTimeCreated,
                        TotalSession = g.TotalSession
                    }).ToList();
                    var savedGroups = await groupService.AssignAttendeeToGroups(groupVMs, attendeeCode);

                    //Get attendeeGroups
                    var savedGroupCodes = savedGroups.Select(g => g.Code).ToList();
                    var savedAttendeeGroups = savedGroups.Select(g => g.AttendeeGroups
                                                                  .First(ag => ag.AttendeeCode == attendeeCode))
                                                    .ToList();

                    //save session if not in db
                    var sessionNetworkVms = (from elementList in attendanceInfo.Groups.Select(g => g.Sessions)
                                             from element in elementList
                                             select element).ToList();
                    var sessionVms = sessionNetworkVms.Select(s => new SessionViewModel()
                    {
                        Name = s.Name,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Status = s.Status,
                        GroupCode = s.GroupCode
                    }).ToList();
                    var savedSessions = await sessionService.AddSessionsIfNotInDbAsync(sessionVms);

                    //save or update records
                    var recordNetworkVms = (from elementList in sessionNetworkVms.Select(s => s.Records)
                                            from element in elementList
                                            select element).ToList();
                    var recordVms = recordNetworkVms.Select(r =>
                    {
                        var svm = sessionNetworkVms.First(s => s.Id == r.SessionId);
                        var savedSession = savedSessions.First(s => s.StartTime == svm.StartTime
                                                                    && s.GroupCode == svm.GroupCode);
                        var savedAttGr = savedAttendeeGroups.First(ag => ag.GroupCode == svm.GroupCode
                                                                && ag.AttendeeCode == attendeeCode);
                        return new RecordViewModel()
                        {
                            AttendeeCode = attendeeCode,
                            SessionName = svm.Name,
                            StartTime = svm.StartTime,
                            EndTime = svm.EndTime,
                            Present = r.Present,
                            UpdateTime = r.UpdateTime,
                            SessionId = savedSession.Id,
                            AttendeeGroupId = savedAttGr.Id
                        };
                    }).ToList();
                    var savedRecords = await recordService.AddOrUpdateRecords(recordVms);

                    //save or update change request
                    var changeRequestNetworkVms = recordNetworkVms.Select(r => r.ChangeRequest);
                    var changeRequestVms = changeRequestNetworkVms.Select(c =>
                    {
                        //update real recordId
                        var savedRecord = savedRecords.First(r => r.Id == c.RecordId);
                        return new ChangeRequestViewModel()
                        {
                            RecordId = savedRecord.Id,
                            Comment = c.Comment,
                            Status = c.Status
                        };
                    }).ToList();
                    await changeRequestService.AddOrUpdateChangeRequests(changeRequestVms);
                    scope.Commit();
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
            return attendee;
        }
    }
}
