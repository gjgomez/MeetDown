using System.ComponentModel;
using System.Linq;
using MeetDown.Events.Core.Extensions;

namespace MeetDown.Events.Core.DataAccess
{
    public class StringSortCriterion<T> : ISortCriterion<T>
    {
        public string SortField { get; set; }

        public ListSortDirection SortDirection { get; set; }

        public StringSortCriterion(string field, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            SortField = field;
            SortDirection = sortDirection;
        }

        public IOrderedQueryable<T> Sort(IQueryable<T> query, bool sorted)
        {
            return sorted ? ((IOrderedQueryable<T>)query).ThenBy(SortField, SortDirection) : query.OrderBy(SortField, SortDirection);
        }
    }
}
