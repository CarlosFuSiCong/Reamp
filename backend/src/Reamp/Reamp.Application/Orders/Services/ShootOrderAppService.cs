using Microsoft.Extensions.Logging;
using Mapster;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Read.Staff;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Repositories;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Listings.Repositories;
using Reamp.Domain.Accounts.Enums;
using PageRequest = Reamp.Domain.Common.Abstractions.PageRequest;

namespace Reamp.Application.Orders.Services
{
    public sealed class ShootOrderAppService : IShootOrderAppService
    {
        private readonly IShootOrderRepository _repo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly IStudioRepository _studioRepo;
        private readonly IListingRepository _listingRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IStaffReadService _staffReadService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ShootOrderAppService> _logger;

        public ShootOrderAppService(
            IShootOrderRepository repo,
            IAgencyRepository agencyRepo,
            IStudioRepository studioRepo,
            IListingRepository listingRepo,
            IStaffRepository staffRepo,
            IStaffReadService staffReadService,
            IUnitOfWork uow,
            ILogger<ShootOrderAppService> logger)
        {
            _repo = repo;
            _agencyRepo = agencyRepo;
            _studioRepo = studioRepo;
            _listingRepo = listingRepo;
            _staffRepo = staffRepo;
            _staffReadService = staffReadService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            var agency = await _agencyRepo.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null)
                throw new ArgumentException($"Agency with ID {dto.AgencyId} does not exist", nameof(dto.AgencyId));

            var studio = await _studioRepo.GetByIdAsync(dto.StudioId, asNoTracking: true, ct);
            if (studio == null)
                throw new ArgumentException($"Studio with ID {dto.StudioId} does not exist", nameof(dto.StudioId));

            var listing = await _listingRepo.GetByIdAsync(dto.ListingId, asNoTracking: true, ct);
            if (listing == null)
                throw new ArgumentException($"Listing with ID {dto.ListingId} does not exist", nameof(dto.ListingId));

            var order = ShootOrder.Place(dto.AgencyId, dto.StudioId, dto.ListingId, currentUserId, dto.Currency ?? "AUD");

            await _repo.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            return order.Adapt<OrderDetailDto>();
        }

        public async Task<IPagedList<OrderListDto>> GetListAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default)
        {
            var orders = await _repo.ListAsync(pageRequest, createdBy: currentUserId, ct: ct);
            var dtos = orders.Items.Adapt<List<OrderListDto>>();
            return new PagedList<OrderListDto>(dtos, orders.TotalCount, orders.Page, orders.PageSize);
        }

        public async Task<OrderDetailDto?> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogDebug("Getting order {OrderId} for user {UserId}", id, currentUserId);

            // Load order with all tasks
            var order = await _repo.GetAggregateAsync(id, ct);

            if (order == null)
                return null;

            // Verify ownership - only creator can view their own orders
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to access order {OrderId} owned by {OwnerId}", 
                    currentUserId, id, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to view this order");
            }

            return order.Adapt<OrderDetailDto>();
        }

        public async Task AddTaskAsync(Guid orderId, AddTaskDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding task {TaskType} to order {OrderId} by user {UserId}", dto.Type, orderId, currentUserId);

            // Load order
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            // Add task
            order.AddTask(dto.Type, dto.Notes, dto.Price);

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully added task to order {OrderId}", orderId);
        }

        public async Task RemoveTaskAsync(Guid orderId, Guid taskId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Removing task {TaskId} from order {OrderId} by user {UserId}", taskId, orderId, currentUserId);

            // Load order
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            // Remove task
            order.RemoveTask(taskId);

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully removed task from order {OrderId}", orderId);
        }

        public async Task AcceptAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Accept();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.MarkScheduled();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task StartAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Start();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Complete();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(Guid orderId, Guid currentUserId, string? reason = null, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Cancel(reason ?? string.Empty);
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task AssignPhotographerAsync(Guid orderId, AssignPhotographerDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            var staff = await _staffRepo.GetByIdAsync(dto.PhotographerId, asNoTracking: true, ct);
            if (staff == null || staff.StudioId != order.StudioId)
                throw new ArgumentException("Invalid photographer for this studio");

            if ((staff.Skills & StaffSkills.Photographer) == 0)
                throw new ArgumentException("Staff member is not a photographer");

            order.AssignPhotographer(dto.PhotographerId);
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task UnassignPhotographerAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.UnassignPhotographer();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<StaffSummaryDto>> GetAvailablePhotographersAsync(Guid orderId, CancellationToken ct = default)
        {
            var order = await _repo.GetByIdAsync(orderId, asNoTracking: true, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            var photographers = await _staffReadService.ListByStudioAsync(
                order.StudioId,
                search: null,
                hasSkill: StaffSkills.Photographer,
                pageRequest: new Reamp.Application.Read.Shared.PageRequest { Page = 1, PageSize = 100 },
                ct: ct);

            return photographers.Items.ToList();
        }

        public async Task SetScheduleAsync(Guid orderId, SetScheduleDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.SetSchedule(dto.ScheduledStartUtc, dto.ScheduledEndUtc, dto.Notes);
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ClearScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.ClearSchedule();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IPagedList<OrderListDto>> GetFilteredListAsync(OrderFilterDto filter, PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default)
        {
            var orders = await _repo.ListFilteredAsync(
                pageRequest,
                agencyId: filter.AgencyId,
                studioId: filter.StudioId,
                listingId: filter.ListingId,
                photographerId: filter.PhotographerId,
                status: filter.Status,
                dateFrom: filter.DateFrom,
                dateTo: filter.DateTo,
                createdBy: currentUserId,
                ct: ct);

            var dtos = orders.Items.Adapt<List<OrderListDto>>();
            return new PagedList<OrderListDto>(dtos, orders.TotalCount, orders.Page, orders.PageSize);
        }

        private class PagedList<T> : IPagedList<T>
        {
            public PagedList(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
            {
                Items = items;
                TotalCount = totalCount;
                Page = page;
                PageSize = pageSize;
            }

            public IReadOnlyList<T> Items { get; }
            public int TotalCount { get; }
            public int Page { get; }
            public int PageSize { get; }
        }
    }
}

