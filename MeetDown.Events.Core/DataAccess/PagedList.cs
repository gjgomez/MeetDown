using System;
using System.Collections.Generic;

namespace MeetDown.Events.Core.DataAccess
{
    public class PagedList<T>
    {
        public IEnumerable<T> Records { get; set; }

        public int? PageNumber { get; }

        public int? PageSize { get; }

        public int TotalRecords { get; }

        public int? TotalPages
        {
            get
            {
                if (PageSize.HasValue)
                {
                    return (int)Math.Ceiling(TotalRecords / (double)PageSize.Value);
                }
                return null;
            }
        }

        public bool HasPreviousPage => (PageNumber.HasValue && PageNumber.Value > 1);

        public bool HasNextPage => (PageNumber.HasValue && PageNumber.Value < TotalPages);

        public PagedList(IEnumerable<T> records, int totalRecords, int? pageNumber, int? pageSize)
        {
            Records = records ?? throw new ArgumentNullException(nameof(records));
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }
    }
}
