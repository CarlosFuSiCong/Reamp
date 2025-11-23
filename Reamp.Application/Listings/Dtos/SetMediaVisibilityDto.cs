using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class SetMediaVisibilityDto
    {
        public Guid MediaId { get; set; }
        public bool IsVisible { get; set; }
    }
}