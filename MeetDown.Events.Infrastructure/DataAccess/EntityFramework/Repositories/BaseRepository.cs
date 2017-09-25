using MeetDown.Events.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MeetDown.Events.Infrastructure.DataAccess.Repositories
{
    internal sealed class BaseRepository
    {
        private readonly DbContext _dbContext;

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TOut> GetAll<TIn, TOut>(
            Expression<Func<TIn, TOut>> projection,
            Expression<Func<TIn, bool>> predicate = null,
            IEnumerable<Expression<Func<TIn, object>>> includes = null,
            IEnumerable<ISortCriterion<TIn>> preSortCriteria = null,
            bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var preProjectionQueryable = _dbContext.Set<TIn>() as IQueryable<TIn>;

            if (includes != null)
            {
                preProjectionQueryable = includes.Aggregate(preProjectionQueryable, (current, include) => current.Include(include));
            }

            if (predicate != null)
            {
                preProjectionQueryable = preProjectionQueryable.Where(predicate);
            }

            if (preSortCriteria != null)
            {
                var sorted = false;
                foreach (var criteria in preSortCriteria)
                {
                    preProjectionQueryable = criteria.Sort(preProjectionQueryable, sorted);
                    sorted = true;
                }
            }

            var postProjectionQueryable = preProjectionQueryable.Select(projection);

            if (!trackInContext)
            {
                postProjectionQueryable = postProjectionQueryable.AsNoTracking();
            }

            return postProjectionQueryable.ToList();
        }

        public PagedList<TOut> GetPage<TIn, TOut>(
            int pageNumber,
            int pageSize,
            Expression<Func<TIn, TOut>> projection,
            Expression<Func<TIn, bool>> predicate = null,
            IEnumerable<Expression<Func<TIn, object>>> includes = null,
            IEnumerable<ISortCriterion<TIn>> sortCriteria = null,
            bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
            }

            var preProjectionQueryable = _dbContext.Set<TIn>() as IQueryable<TIn>;

            if (predicate != null)
            {
                preProjectionQueryable = preProjectionQueryable.Where(predicate);
            }

            if (includes != null)
            {
                preProjectionQueryable = includes.Aggregate(preProjectionQueryable, (current, include) => current.Include(include));
            }

            if (sortCriteria != null)
            {
                var sorted = false;
                foreach (var criteria in sortCriteria)
                {
                    preProjectionQueryable = criteria.Sort(preProjectionQueryable, sorted);
                    sorted = true;
                }
            }

            var postProjectionQueryable = preProjectionQueryable.Select(projection);

            if (!trackInContext)
            {
                postProjectionQueryable = postProjectionQueryable.AsNoTracking();
            }

            // store the total number of records before getting the smaller paged subset
            var recordCount = postProjectionQueryable.Count();
            var records = pageNumber > 1 ? postProjectionQueryable.Skip((pageNumber - 1) * pageSize).Take(pageSize) : postProjectionQueryable.Take(pageSize);
            var recordList = records.ToList();

            return new PagedList<TOut>(recordList, recordCount, pageNumber, pageSize);
        }

        public TOut Get<TIn, TOut>(
            Expression<Func<TIn, TOut>> projection,
            Expression<Func<TIn, bool>> predicate = null,
            IEnumerable<Expression<Func<TIn, object>>> includes = null,
            bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var preProjectionQueryable = _dbContext.Set<TIn>() as IQueryable<TIn>;

            if (includes != null)
            {
                preProjectionQueryable = includes.Aggregate(preProjectionQueryable, (current, include) => current.Include(include));
            }

            if (predicate != null)
            {
                preProjectionQueryable = preProjectionQueryable.Where(predicate);
            }

            var postProjectionQueryable = preProjectionQueryable.Select(projection);

            if (!trackInContext)
            {
                postProjectionQueryable = postProjectionQueryable.AsNoTracking();
            }

            return postProjectionQueryable.SingleOrDefault();
        }
    }
}
