using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class ReorderMediaDto
    {
        public List<SortOrderItemDto> Items { get; set; } = new();
    }
}