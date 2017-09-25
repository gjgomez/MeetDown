using MeetDown.Events.Core.DataAccess;
using MeetDown.Events.Core.DataAccess.Repositories;
using MeetDown.Events.Core.Entities;
using MeetDown.Events.Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MeetDown.Events.Infrastructure.DataAccess.EntityFramework.Repositories
{
    public class MeetRepository : IMeetRepository
    {
        private readonly MeetDbContext _meetDbContext;
        private readonly BaseRepository _baseRepository;
        private readonly IDistributedCache _cacheStore;
        private readonly bool _cacheEnabled;

        public MeetRepository(IConfiguration configuration, IDistributedCache cache)
        {
            _meetDbContext = new MeetDbContext(configuration);
            _baseRepository = new BaseRepository(_meetDbContext);
            _cacheStore = cache;
            _cacheEnabled = bool.Parse(configuration["DistributedCache:Enabled"]);
        }

        public IEnumerable<Group> GetGroups()
        {
            IEnumerable<Group> groups;

            if (_cacheEnabled)
            {
                var cacheJson = _cacheStore.GetString(CacheKey.GetGroups.ToString());

                if (string.IsNullOrWhiteSpace(cacheJson))
                {
                    // not in cache, so get from DB
                    groups = _baseRepository.GetAll<Group, Group>(projection: x => x);
                    _cacheStore.SetString(CacheKey.GetGroups.ToString(), JsonConvert.SerializeObject(groups), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                }
                else
                {
                    groups = JsonConvert.DeserializeObject<IEnumerable<Group>>(cacheJson);
                }
            }
            else
            {
                groups = _baseRepository.GetAll<Group, Group>(projection: x => x);
            }

            return groups;
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
            var groupEntitiesModified = _meetDbContext.ChangeTracker.Entries<Group>().Any(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted);

            var changesSaved = _meetDbContext.SaveChanges() > 0;

            if (_cacheEnabled && groupEntitiesModified && changesSaved)
            {
                _cacheStore.Remove(CacheKey.GetGroups.ToString());
            }

            return changesSaved;
        }
    }
}
