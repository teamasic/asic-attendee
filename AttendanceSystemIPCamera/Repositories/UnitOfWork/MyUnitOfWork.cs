using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.SessionService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories.UnitOfWork
{
    public class MyUnitOfWork : UnitOfWork
    {
        
        public MyUnitOfWork(DbContext dbContext) : base(dbContext)
        {
        }
        public IRepository<T> GetRepository<T>() where T : class, BaseEntity
        {
            return new Repository<T>(DbContext);
        }
        #region Repository

        private IGroupRepository groupRepository;
        private ISessionRepository sessionRepository;
        private IAttendeeRepository attendeeRepository;
        private IRecordRepository recordRepository;

        public IRecordRepository RecordRepository
        {
            get
            {
                if (recordRepository == null)
                {
                    recordRepository = new RecordRepository(DbContext);
                }
                return recordRepository;
            }
        }
        public IAttendeeRepository AttendeeRepository
        {
            get
            {
                if (attendeeRepository == null)
                {
                    attendeeRepository = new AttendeeRepository(DbContext);
                }
                return attendeeRepository;
            }
        }

        public IGroupRepository GroupRepository
        {
            get
            {
                if (groupRepository == null)
                {
                    groupRepository = new GroupRepository(DbContext);
                }
                return groupRepository;
            }
        }
        public ISessionRepository SessionRepository
        {
            get
            {
                if (sessionRepository == null)
                {
                    sessionRepository = new SessionRepository(DbContext);
                }
                return sessionRepository;
            }
        }
        #endregion

        #region Service
        private IAttendeeService attendeeService;
        private IGroupService groupService;
        private ISessionService sessionService;
        private IAttendeeNetworkService attendeeNetworkService;
        private IRecordService recordService;

        public IRecordService RecordService
        {
            get
            {
                if (recordService == null)
                {
                    recordService = new RecordService(this);
                }
                return recordService;
            }
        }

        public IAttendeeNetworkService AttendeeNetworkService
        {
            get
            {
                if (attendeeNetworkService == null)
                {
                    attendeeNetworkService = new AttendeeNetworkService(this);
                }
                return attendeeNetworkService;
            }
        }

        public ISessionService SessionService
        {
            get
            {
                if (sessionService == null)
                {
                    sessionService = new SessionService(this);
                }
                return sessionService;
            }
        }

        public IGroupService GroupService
        {
            get
            {
                if (groupService == null)
                {
                    groupService = new GroupService(this);
                }
                return groupService;
            }
        }


        public IAttendeeService AttendeeService
        {
            get
            {
                if (attendeeService == null)
                {
                    attendeeService = new AttendeeService(this);
                }
                return attendeeService;
            }
        }


        #endregion
    }
}
