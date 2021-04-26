using System;
using System.Collections.Generic;

namespace ReuseventoryApi.Models.DTO
{
    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }

    public class PagedResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstResult
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastResult
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }
}