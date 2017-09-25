﻿using MeetDown.Events.Core.DataAccess;
using MeetDown.Events.Core.DataAccess.Repositories;
using MeetDown.Events.Core.Entities;
using MeetDown.Events.Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace MeetDown.Events.Infrastructure.DataAccess.EntityFramework.Repositories
{
    public class MeetRepository : IMeetRepository
    {
        private readonly MeetDbContext _meetDbContext;
        private readonly BaseRepository _baseRepository;

        public MeetRepository(IConfiguration configuration)
        {
            _meetDbContext = new MeetDbContext(configuration);
            _baseRepository = new BaseRepository(_meetDbContext);
        }

        public IEnumerable<Group> GetGroups()
        {
            return _baseRepository.GetAll<Group, Group>(projection: x => x);
        }

        public PagedList<Group> GetGroups(int pageNumber, int pageSize, IEnumerable<ISortCriterion<Group>> sortCriteria)
        {
            return _baseRepository.GetPage(
                pageNumber: pageNumber,
                pageSize: pageSize,
                projection: x => x,
                sortCriteria: sortCriteria,
                trackInContext: false);
        }

        public bool GroupExists(int groupId)
        {
            return _meetDbContext.Groups.Any(x => x.Id == groupId);
        }

        public Group GetGroup(int id, bool includeMeets = false)
        {
            var additionalProperties = new List<Expression<Func<Group, object>>>();

            if (includeMeets)
                additionalProperties.Add(x => x.Meets);

            return _baseRepository.Get<Group, Group>(projection: x => x, predicate: x => x.Id == id, includes: additionalProperties);
        }

        public void AddGroup(Group group)
        {
            _meetDbContext.Groups.Add(group);
        }

        public void RemoveGroup(Group group)
        {
            _meetDbContext.Groups.Remove(group);
        }

        public PagedList<Meet> GetMeets(int groupId, int pageNumber, int pageSize, IEnumerable<ISortCriterion<Meet>> sortCriteria)
        {
            return _baseRepository.GetPage(
                pageNumber: pageNumber,
                pageSize: pageSize,
                projection: x => x,
                predicate: x => x.GroupId == groupId,
                sortCriteria: sortCriteria,
                trackInContext: false);
        }

        public Meet GetMeet(int groupId, int id)
        {
            return _baseRepository.Get<Meet, Meet>(projection: x => x, predicate: x => x.GroupId == groupId && x.Id == id);
        }

        public void AddMeetToGroup(int groupId, Meet meet)
        {
            var group = GetGroup(groupId, true);

            if (group == null)
            {
                throw new ArgumentException($"Cannot find Group with id {groupId}", nameof(groupId));
            }

            group.Meets.Add(meet);
        }

        public void RemoveMeet(Meet meet)
        {
            _meetDbContext.Meets.Remove(meet);
        }

        public bool Save()
        {
            return _meetDbContext.SaveChanges() > 0;
        }

    }
}
