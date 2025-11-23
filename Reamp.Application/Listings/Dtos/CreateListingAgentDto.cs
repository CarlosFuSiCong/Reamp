using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class CreateListingAgentDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Team { get; set; }
        public string? Avatar { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}