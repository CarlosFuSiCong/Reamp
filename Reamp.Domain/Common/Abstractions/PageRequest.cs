using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Abstractions
{
    public sealed class PageRequest
    {
        public int Page { get; }
        public int PageSize { get; }

        public PageRequest(int page = 1, int pageSize = 20)
        {
            Page = page < 1 ? 1 : page;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;
            PageSize = pageSize;
        }

        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }
}