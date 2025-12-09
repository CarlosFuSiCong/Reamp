using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Shared
{
    public sealed class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = new List<T>();
        public int Total { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}