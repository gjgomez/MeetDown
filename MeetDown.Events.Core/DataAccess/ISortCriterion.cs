using System.ComponentModel;
using System.Linq;

namespace MeetDown.Events.Core.DataAccess
{
    public interface ISortCriterion<T>
    {
        ListSortDirection SortDirection { get; set; }

        IOrderedQueryable<T> Sort(IQueryable<T> query, bool sorted);
    }
}
