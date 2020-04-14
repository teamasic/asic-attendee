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
        public Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel);
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
            if (record.Present == viewModel.Present)
            {
                throw new AppException(HttpStatusCode.BadRequest, ErrorMessage.CHANGE_REQUEST_INVALID);
            }
            var newRequest = new ChangeRequest
            {
                Record = record,
                Comment = viewModel.Comment,
                Status = ChangeRequestStatus.UNRESOLVED
            };
            await changeRequestRepository.Add(newRequest);
            unitOfWork.Commit();
            unitOfWork.AttendeeNetworkService.CreateChangeRequest(viewModel);
            return newRequest;
        }

        public async Task<IEnumerable<ChangeRequest>> GetAll(SearchChangeRequestViewModel viewModel)
        {
            return await changeRequestRepository.GetAll(viewModel);
        }

        public async Task<ChangeRequest> Process(ProcessChangeRequestViewModel viewModel)
        {
            var changeRequest = await changeRequestRepository.GetByIdSimple(viewModel.ChangeRequestId);
            if (viewModel.Approved)
            {
                changeRequest.Record.Present = true;
                changeRequest.Status = ChangeRequestStatus.APPROVED;
            }
            else
            {
                changeRequest.Record.Present = false;
                changeRequest.Status = ChangeRequestStatus.REJECTED;
            }
            changeRequestRepository.Update(changeRequest);
            unitOfWork.Commit();
            return changeRequest;
        }
 
        public async Task<List<ChangeRequest>> AddOrUpdateChangeRequests(List<ChangeRequestViewModel> changeRequestVms)
        {
            var changeRequestReturn = new List<ChangeRequest>();

            var recordIds = changeRequestVms.Select(c => c.RecordId).ToList();
            var changeReqsInDb = changeRequestRepository.GetByRecordIds(recordIds);
            var recordIdsInChangeReqsInDb = changeReqsInDb.Select(c => c.RecordId).ToList();
            var changeReqsNotInDb = changeRequestVms.Where(c => !recordIdsInChangeReqsInDb.Contains(c.RecordId)).ToList();

            //add change request not exist
            if(changeReqsNotInDb != null && changeReqsNotInDb.Count > 0)
            {
                var changeReqs = mapper.ProjectTo<ChangeRequestViewModel, ChangeRequest>(changeReqsNotInDb);
                await changeRequestRepository.Add(changeReqs);
                unitOfWork.Commit();
                changeRequestReturn.AddRange(changeReqs);
            }
            if(changeReqsInDb != null && changeReqsInDb.Count > 0)
            {
                changeReqsInDb.ForEach(c =>
                {
                    var changeReqVm = changeRequestVms.First(c => c.RecordId == c.RecordId);
                    c.Status = changeReqVm.Status;
                    c.Comment = changeReqVm.Comment;
                });
                unitOfWork.Commit();
                changeRequestReturn.AddRange(changeReqsInDb);
            }

            return changeRequestReturn;
        }
    }
}
