using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Accounts.Studios.Dtos
{
    public sealed class CreateStudioDto
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? LogoUrl { get; init; }

        public Guid CreatedBy { get; init; }
        public string ContactEmail { get; init; } = string.Empty;
        public string ContactPhone { get; init; } = string.Empty;

        public AddressDto Address { get; init; } = new();
    }
}