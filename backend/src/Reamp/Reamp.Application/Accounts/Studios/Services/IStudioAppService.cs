using Reamp.Application.Accounts.Studios.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Accounts.Studios.Services
{
    public interface IStudioAppService
    {
        Task<Guid> CreateAsync(CreateStudioDto dto, CancellationToken ct);
        Task<IPagedList<Studio>> ListAsync(int page, int pageSize, string? search, CancellationToken ct);
        Task<Studio?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Studio?> GetBySlugAsync(string slug, CancellationToken ct);
    }
}