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
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using System.Net;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.NetworkService;

namespace AttendanceSystemIPCamera.Services.ChangeRequestService
{
    public interface IChangeRequestService : IBaseService<ChangeRequest>
    {
        public Task<ChangeRequest> Add(CreateChangeRequestViewModel viewModel);
        public Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel);
        Task<List<ChangeRequest>> AddOrUpdateChangeRequests(List<ChangeRequestViewModel> changeRequestVms);
    }

    public class ChangeRequestService: BaseService<ChangeRequest>, IChangeRequestService
    {
        private readonly IChangeRequestRepository changeRequestRepository;
        private readonly IRecordRepository recordRepository;

        //private readonly IAttendeeNetworkService attendeeNetworkService;

        private readonly IMapper mapper;

        public ChangeRequestService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            changeRequestRepository = unitOfWork.ChangeRequestRepository;
            recordRepository = unitOfWork.RecordRepository;

            //this.attendeeNetworkService = unitOfWork.AttendeeNetworkService;

            this.mapper = unitOfWork.mapper;
        }

        public async Task<ChangeRequest> Add(CreateChangeRequestViewModel viewModel)
        {
            var record = await recordRepository.GetById(viewModel.RecordId);
            if (record == null)
            {
                throw new AppException(HttpStatusCode.NotFound, ErrorMessage.NOT_FOUND_RECORD_WITH_ID, viewModel.RecordId);
            }
            /*
            if (record.Present == viewModel.Present)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.CHANGE_REQUEST_INVALID);
            }
            */
            var newRequest = new ChangeRequest
            {
                Record = record,
                Comment = viewModel.Comment,
                Status = ChangeRequestStatus.UNRESOLVED,
                DateSubmitted = DateTime.Now
            };
            await changeRequestRepository.Add(newRequest);
            unitOfWork.Commit();
            await unitOfWork.AttendeeNetworkService.CreateChangeRequest(new CreateChangeRequestNetworkViewModel { 
                AttendeeCode = record.AttendeeCode,
                GroupCode = record.Session.GroupCode,
                Comment = viewModel.Comment,
                StartTime = record.StartTime
            });
            return newRequest;
        }

        public async Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel)
        {
            return await changeRequestRepository.GetAll(viewModel);
        }
 
        public async Task<List<ChangeRequest>> AddOrUpdateChangeRequests(List<ChangeRequestViewModel> changeRequestVms)
        {
            var recordIds = changeRequestVms.Select(c => c.RecordId).ToList();
            var changeReqsInDb = changeRequestRepository.GetByRecordIds(recordIds);

            changeReqsInDb.ForEach(c =>
            {
                var changeReqVm = changeRequestVms.First(crVM => crVM.RecordId == c.RecordId);
                c.Status = changeReqVm.Status;
                c.Comment = changeReqVm.Comment;
            });
            unitOfWork.Commit();
            return changeReqsInDb;
        }
    }
}
