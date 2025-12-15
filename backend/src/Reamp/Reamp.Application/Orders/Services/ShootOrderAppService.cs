using Microsoft.Extensions.Logging;
using Mapster;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Read.Staff;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Repositories;
using Reamp.Domain.Shoots.Enums;
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
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ShootOrderAppService> _logger;

        public ShootOrderAppService(
            IShootOrderRepository repo,
            IAgencyRepository agencyRepo,
            IStudioRepository studioRepo,
            IListingRepository listingRepo,
            IStaffRepository staffRepo,
            IStaffReadService staffReadService,
            IUserProfileRepository userProfileRepo,
            IUnitOfWork uow,
            ILogger<ShootOrderAppService> logger)
        {
            _repo = repo;
            _agencyRepo = agencyRepo;
            _studioRepo = studioRepo;
            _listingRepo = listingRepo;
            _staffRepo = staffRepo;
            _staffReadService = staffReadService;
            _userProfileRepo = userProfileRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            var agency = await _agencyRepo.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null)
                throw new ArgumentException($"Agency with ID {dto.AgencyId} does not exist", nameof(dto.AgencyId));

            // StudioId is optional - if provided, validate it exists
            if (dto.StudioId.HasValue)
            {
                var studio = await _studioRepo.GetByIdAsync(dto.StudioId.Value, asNoTracking: true, ct);
                if (studio == null)
                    throw new ArgumentException($"Studio with ID {dto.StudioId} does not exist", nameof(dto.StudioId));
            }

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

            // Check if user is the creator OR the assigned photographer
            var isCreator = order.CreatedBy == currentUserId;
            var isAssignedPhotographer = false;

            if (order.AssignedPhotographerId.HasValue)
            {
                var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
                if (userProfile != null)
                {
                    var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
                    if (staff != null && staff.Id == order.AssignedPhotographerId.Value)
                    {
                        isAssignedPhotographer = true;
                    }
                }
            }

            // Verify ownership - only creator or assigned photographer can view
            if (!isCreator && !isAssignedPhotographer)
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

            // Check if user is the creator OR the assigned photographer
            var isCreator = order.CreatedBy == currentUserId;
            var isAssignedPhotographer = false;

            if (order.AssignedPhotographerId.HasValue)
            {
                var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
                if (userProfile != null)
                {
                    var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
                    if (staff != null && staff.Id == order.AssignedPhotographerId.Value)
                    {
                        isAssignedPhotographer = true;
                    }
                }
            }

            if (!isCreator && !isAssignedPhotographer)
                throw new UnauthorizedAccessException("You do not have permission to start this order");

            order.Start();
            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Check if user is the creator OR the assigned photographer
            var isCreator = order.CreatedBy == currentUserId;
            var isAssignedPhotographer = false;

            if (order.AssignedPhotographerId.HasValue)
            {
                var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
                if (userProfile != null)
                {
                    var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
                    if (staff != null && staff.Id == order.AssignedPhotographerId.Value)
                    {
                        isAssignedPhotographer = true;
                    }
                }
            }

            if (!isCreator && !isAssignedPhotographer)
                throw new UnauthorizedAccessException("You do not have permission to complete this order");

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

            // If order doesn't have a studio assigned yet, return empty list
            if (!order.StudioId.HasValue)
                return new List<StaffSummaryDto>();

            var photographers = await _staffReadService.ListByStudioAsync(
                order.StudioId.Value,
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

        // Photographer-specific operations
        public async Task<IPagedList<OrderListDto>> GetAvailableOrdersAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Getting available orders for user {UserId}", currentUserId);

            // Get the current user's profile and staff record
            var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
            if (userProfile == null)
            {
                _logger.LogWarning("No UserProfile found for ApplicationUserId: {UserId}", currentUserId);
                return new PagedList<OrderListDto>(new List<OrderListDto>(), 0, pageRequest.Page, pageRequest.PageSize);
            }

            var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
            if (staff == null)
            {
                _logger.LogWarning("User {UserId} (UserProfile {ProfileId}) is not a staff member", currentUserId, userProfile.Id);
                return new PagedList<OrderListDto>(new List<OrderListDto>(), 0, pageRequest.Page, pageRequest.PageSize);
            }

            // Check if staff has photographer skills
            if ((staff.Skills & StaffSkills.Photographer) == 0)
            {
                _logger.LogWarning("Staff member {StaffId} does not have photographer skills", staff.Id);
                return new PagedList<OrderListDto>(new List<OrderListDto>(), 0, pageRequest.Page, pageRequest.PageSize);
            }

            // Get orders that:
            // 1. Have the same studioId as the photographer
            // 2. Status is Placed or Accepted (not yet assigned to a photographer)
            // 3. No photographer assigned (AssignedPhotographerId is null)
            var orders = await _repo.ListFilteredAsync(
                pageRequest,
                studioId: staff.StudioId,
                photographerId: null, // Only unassigned orders
                status: null, // We'll filter by multiple statuses
                ct: ct);

            // Filter to only Placed or Accepted status
            var filteredOrders = orders.Items
                .Where(o => o.Status == ShootOrderStatus.Placed || 
                           o.Status == ShootOrderStatus.Accepted)
                .Where(o => !o.AssignedPhotographerId.HasValue)
                .ToList();

            var dtos = filteredOrders.Adapt<List<OrderListDto>>();
            return new PagedList<OrderListDto>(dtos, filteredOrders.Count, pageRequest.Page, pageRequest.PageSize);
        }

        public async Task<IPagedList<OrderListDto>> GetPhotographerOrdersAsync(PageRequest pageRequest, Guid currentUserId, ShootOrderStatus? status = null, CancellationToken ct = default)
        {
            _logger.LogInformation("Getting photographer orders for user {UserId} with status {Status}", currentUserId, status);

            // Get the current user's profile and staff record
            var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
            if (userProfile == null)
            {
                _logger.LogWarning("No UserProfile found for ApplicationUserId: {UserId}", currentUserId);
                return new PagedList<OrderListDto>(new List<OrderListDto>(), 0, pageRequest.Page, pageRequest.PageSize);
            }

            var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
            if (staff == null)
            {
                _logger.LogWarning("User {UserId} (UserProfile {ProfileId}) is not a staff member", currentUserId, userProfile.Id);
                return new PagedList<OrderListDto>(new List<OrderListDto>(), 0, pageRequest.Page, pageRequest.PageSize);
            }

            // Get orders assigned to this photographer
            var orders = await _repo.ListFilteredAsync(
                pageRequest,
                photographerId: staff.Id,
                status: status,
                ct: ct);

            var dtos = orders.Items.Adapt<List<OrderListDto>>();
            return new PagedList<OrderListDto>(dtos, orders.TotalCount, orders.Page, orders.PageSize);
        }

        public async Task AcceptOrderAsPhotographerAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Photographer accepting order {OrderId} as user {UserId}", orderId, currentUserId);

            // Get the current user's profile and staff record
            var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
            if (userProfile == null)
            {
                _logger.LogWarning("No UserProfile found for ApplicationUserId: {UserId}", currentUserId);
                throw new UnauthorizedAccessException("User profile not found");
            }

            var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
            if (staff == null)
            {
                _logger.LogWarning("User {UserId} (UserProfile {ProfileId}) is not a staff member", currentUserId, userProfile.Id);
                throw new UnauthorizedAccessException("You must be a staff member to accept orders");
            }

            // Check if staff has photographer skills
            if ((staff.Skills & StaffSkills.Photographer) == 0)
            {
                _logger.LogWarning("Staff member {StaffId} does not have photographer skills", staff.Id);
                throw new UnauthorizedAccessException("You must be a photographer to accept orders");
            }

            // Load the order
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Verify the order belongs to the same studio
            if (order.StudioId != staff.StudioId)
            {
                _logger.LogWarning("Order {OrderId} belongs to studio {StudioId}, but photographer belongs to studio {PhotographerStudioId}", 
                    orderId, order.StudioId, staff.StudioId);
                throw new UnauthorizedAccessException("You can only accept orders from your studio");
            }

            // Verify the order is available to be accepted
            if (order.Status != ShootOrderStatus.Placed && 
                order.Status != ShootOrderStatus.Accepted)
            {
                throw new InvalidOperationException($"Order cannot be accepted in status {order.Status}");
            }

            if (order.AssignedPhotographerId.HasValue)
            {
                throw new InvalidOperationException("Order is already assigned to a photographer");
            }

            // Assign the photographer to the order
            order.AssignPhotographer(staff.Id);
            
            // Accept the order if it's still in Placed status
            if (order.Status == ShootOrderStatus.Placed)
            {
                order.Accept();
            }

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Order {OrderId} accepted by photographer {StaffId}", orderId, staff.Id);
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

