using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class SortOrderItemDto
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
    }
}