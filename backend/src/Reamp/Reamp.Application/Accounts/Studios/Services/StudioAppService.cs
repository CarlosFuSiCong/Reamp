using Reamp.Application.Accounts.Studios.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Accounts.Studios.Services
{
    public sealed class StudioAppService : IStudioAppService
    {
        private readonly IStudioRepository _studios;
        private readonly IUnitOfWork _uow;

        public StudioAppService(IStudioRepository studios, IUnitOfWork uow)
        {
            _studios = studios;
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(CreateStudioDto dto, CancellationToken ct)
        {
            var slugFromName = Slug.From(dto.Name);

            if (await _studios.ExistsBySlugAsync(slugFromName, ct))
                throw new InvalidOperationException($"Studio slug '{slugFromName.Value}' already exists.");

            var address = new Address(
                dto.Address.Line1,
                dto.Address.City,
                dto.Address.State,
                dto.Address.Postcode,
                dto.Address.Country,
                dto.Address.Line2,
                dto.Address.Latitude,
                dto.Address.Longitude
            );

            var studio = Studio.Create(
                name: dto.Name,
                createdBy: dto.CreatedBy,
                contactEmail: dto.ContactEmail,
                contactPhone: dto.ContactPhone,
                description: dto.Description,
                address: address
            );

            await _studios.AddAsync(studio, ct);
            await _uow.SaveChangesAsync(ct);

            return studio.Id;
        }

        public Task<IPagedList<Studio>> ListAsync(int page, int pageSize, string? search, CancellationToken ct)
            => _studios.ListAsync(new PageRequest(page, pageSize), search, ct);

        public Task<Studio?> GetBySlugAsync(string slug, CancellationToken ct)
            => _studios.GetBySlugAsync(Slug.From(slug), asNoTracking: true, ct);
    }
}