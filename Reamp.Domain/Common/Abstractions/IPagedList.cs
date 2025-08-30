using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Abstractions
{
    public interface IPagedList<out T>
    {
        int Page { get; }
        int PageSize { get; }
        int TotalCount { get; }
        IReadOnlyList<T> Items { get; }
    }

    public sealed class PagedList<T> : IPagedList<T>
    {
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public IReadOnlyList<T> Items { get; }

        public PagedList(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}