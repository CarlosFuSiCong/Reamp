using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Shared
{
    public sealed class PageRequest
    {
        public const int MaxPageSize = 100;

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? SortBy { get; init; }
        public bool Desc { get; init; }

        public PageRequest Normalize()
        {
            var p = Page <= 0 ? 1 : Page;
            var s = PageSize <= 0 ? 20 : (PageSize > MaxPageSize ? MaxPageSize : PageSize);
            return new PageRequest { Page = p, PageSize = s, SortBy = SortBy, Desc = Desc };
        }
    }
}