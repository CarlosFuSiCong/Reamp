using Microsoft.Extensions.Logging;
using Mapster;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Read.Staff;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Orders.Entities;
using Reamp.Domain.Orders.Repositories;
using Reamp.Domain.Orders.Enums;
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
        private readonly IAgentRepository _agentRepo;
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
            IAgentRepository agentRepo,
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
            _agentRepo = agentRepo;
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
            // Auto-populate AgencyId if not provided
            if (dto.AgencyId == Guid.Empty)
            {
                var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
                if (userProfile == null)
                {
                    _logger.LogWarning("No UserProfile found for ApplicationUserId: {UserId}", currentUserId);
                    throw new ArgumentException("User profile not found", nameof(currentUserId));
                }

                var agent = await _agentRepo.GetByUserProfileIdAsync(userProfile.Id, ct);
                if (agent == null)
                {
                    _logger.LogWarning("User {UserId} (UserProfile {ProfileId}) has no agent record", currentUserId, userProfile.Id);
                    throw new InvalidOperationException("You must be part of an agency to create orders. Please submit an agency application first.");
                }

                dto.AgencyId = agent.AgencyId;
                _logger.LogInformation("Auto-populated AgencyId {AgencyId} for user {UserId}", dto.AgencyId, currentUserId);
            }

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

            var order = ShootOrder.Place(dto.AgencyId, dto.StudioId, dto.ListingId, currentUserId, dto.Title, dto.Currency ?? "AUD");

            // Set schedule if provided
            if (dto.ScheduledStartUtc.HasValue)
            {
                order.SetSchedule(dto.ScheduledStartUtc.Value, dto.ScheduledEndUtc);
            }

            await _repo.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            return order.Adapt<OrderDetailDto>();
        }

        public async Task<IPagedList<OrderListDto>> GetListAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default)
        {
            var orders = await _repo.ListAsync(pageRequest, createdBy: currentUserId, ct: ct);
            var dtos = await EnrichOrderListDtosAsync(orders.Items, ct);
            return new PagedList<OrderListDto>(dtos, orders.TotalCount, orders.Page, orders.PageSize);
        }

        private async Task<List<OrderListDto>> EnrichOrderListDtosAsync(IEnumerable<ShootOrder> orders, CancellationToken ct)
        {
            var ordersList = orders.ToList();
            if (!ordersList.Any()) return new List<OrderListDto>();

            // Get all unique IDs
            var listingIds = ordersList.Select(x => x.ListingId).Distinct().ToList();
            var studioIds = ordersList.Where(x => x.StudioId.HasValue).Select(x => x.StudioId!.Value).Distinct().ToList();
            var agencyIds = ordersList.Select(x => x.AgencyId).Distinct().ToList();

            // Load related entities sequentially (DbContext cannot handle concurrent operations)
            var listingsArray = await Task.WhenAll(listingIds.Select(id => _listingRepo.GetByIdAsync(id, asNoTracking: true, ct)));
            var listings = listingsArray.Where(x => x != null).ToDictionary(x => x!.Id);

            var studiosArray = await Task.WhenAll(studioIds.Select(id => _studioRepo.GetByIdAsync(id, asNoTracking: true, ct)));
            var studios = studiosArray.Where(x => x != null).ToDictionary(x => x!.Id);

            var agenciesArray = await Task.WhenAll(agencyIds.Select(id => _agencyRepo.GetByIdAsync(id, asNoTracking: true, ct)));
            var agencies = agenciesArray.Where(x => x != null).ToDictionary(x => x!.Id);

            // Map and enrich
            return ordersList.Select(order =>
            {
                var dto = order.Adapt<OrderListDto>();
                dto.TaskCount = order.Tasks.Count;
                
                _logger.LogDebug("Order {OrderId} has {TaskCount} tasks in collection", order.Id, order.Tasks.Count);

                if (listings.TryGetValue(order.ListingId, out var listing))
                {
                    dto.ListingTitle = listing.Title;
                    dto.ListingAddress = $"{listing.Address.Line1}, {listing.Address.City}";
                }

                if (order.StudioId.HasValue && studios.TryGetValue(order.StudioId.Value, out var studio))
                {
                    dto.StudioName = studio!.Name;
                }

                if (agencies.TryGetValue(order.AgencyId, out var agency))
                {
                    dto.AgencyName = agency!.Name;
                }

                return dto;
            }).ToList();
        }

        public async Task<OrderDetailDto?> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogDebug("Getting order {OrderId} for user {UserId}", id, currentUserId);

            // Load order with all tasks (with tracking for view)
            var order = await _repo.GetAggregateAsync(id, asNoTracking: false, ct);

            if (order == null)
                return null;

            // Check if user is the creator
            var isCreator = order.CreatedBy == currentUserId;
            
            // Check if user is staff and can view this order
            var canViewAsStaff = false;
            var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(currentUserId, includeDeleted: false, asNoTracking: true, ct);
            if (userProfile != null)
            {
                var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
                if (staff != null)
                {
                    // Staff can view if:
                    // 1. They are assigned to the order
                    if (order.AssignedPhotographerId.HasValue && staff.Id == order.AssignedPhotographerId.Value)
                    {
                        canViewAsStaff = true;
                        _logger.LogDebug("Staff {StaffId} is assigned to order {OrderId}", staff.Id, id);
                    }
                    // 2. Order belongs to their studio
                    else if (order.StudioId.HasValue && staff.StudioId == order.StudioId)
                    {
                        canViewAsStaff = true;
                        _logger.LogDebug("Order {OrderId} belongs to staff's studio {StudioId}", id, staff.StudioId);
                    }
                    // 3. It's a marketplace order (StudioId is null) and they can claim it
                    else if (!order.StudioId.HasValue && (order.Status == Domain.Shoots.Enums.ShootOrderStatus.Placed || order.Status == Domain.Shoots.Enums.ShootOrderStatus.Accepted))
                    {
                        canViewAsStaff = true;
                        _logger.LogDebug("Order {OrderId} is a marketplace order available for staff {StaffId}", id, staff.Id);
                    }
                }
            }

            // Verify permission - creator or staff who can access
            if (!isCreator && !canViewAsStaff)
            {
                _logger.LogWarning("User {UserId} attempted to access order {OrderId} without permission. Creator: {CreatedBy}, StudioId: {StudioId}, AssignedPhotographer: {AssignedPhotographerId}", 
                    currentUserId, id, order.CreatedBy, order.StudioId, order.AssignedPhotographerId);
                throw new UnauthorizedAccessException("You do not have permission to view this order");
            }

            return order.Adapt<OrderDetailDto>();
        }

        public async Task AddTaskAsync(Guid orderId, AddTaskDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding task {TaskType} to order {OrderId} by user {UserId}", dto.Type, orderId, currentUserId);

            // Load order WITH tracking to verify ownership
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}",
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            // Add task - this will modify order properties (TotalAmount, UpdatedAtUtc) and add task to collection
            order.AddTask(dto.Type, dto.Notes, dto.Price);

            // Save changes - order will be updated (TotalAmount, UpdatedAtUtc) and task will be inserted
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully added task to order {OrderId}", orderId);
        }

        public async Task RemoveTaskAsync(Guid orderId, Guid taskId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Removing task {TaskId} from order {OrderId} by user {UserId}", taskId, orderId, currentUserId);

            // Load order without tracking
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
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

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully removed task from order {OrderId}", orderId);
        }

        public async Task AcceptAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Accept();
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.MarkScheduled();
            await _uow.SaveChangesAsync(ct);
        }

        public async Task StartAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
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
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
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
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(Guid orderId, Guid currentUserId, string? reason = null, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.Cancel(reason ?? string.Empty);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task AssignPhotographerAsync(Guid orderId, AssignPhotographerDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
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
            await _uow.SaveChangesAsync(ct);
        }

        public async Task UnassignPhotographerAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.UnassignPhotographer();
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
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.SetSchedule(dto.ScheduledStartUtc, dto.ScheduledEndUtc, dto.Notes);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ClearScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to modify this order");

            order.ClearSchedule();
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

            var dtos = await EnrichOrderListDtosAsync(orders.Items, ct);
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

            // Get orders that are available for this photographer:
            // 1. Marketplace orders (StudioId is null) - any photographer can claim
            // 2. Orders assigned to this photographer's studio (StudioId == staff.StudioId)
            // Both types must be:
            // - Status is Placed or Accepted (not yet started)
            // - No photographer assigned yet (AssignedPhotographerId is null)
            var orders = await _repo.ListFilteredAsync(
                pageRequest,
                studioId: null, // Don't filter by studio yet, we'll do it in memory
                photographerId: null, // Only unassigned orders
                status: null, // We'll filter by multiple statuses
                ct: ct);

            _logger.LogInformation("ListFilteredAsync returned {Count} orders for page {Page}", 
                orders.Items.Count, pageRequest.Page);

            // Filter to:
            // 1. Placed or Accepted status
            // 2. No photographer assigned
            // 3. Either marketplace order (no studio) OR assigned to this photographer's studio
            var filteredOrders = orders.Items
                .Where(o => o.Status == ShootOrderStatus.Placed || 
                           o.Status == ShootOrderStatus.Accepted)
                .Where(o => !o.AssignedPhotographerId.HasValue)
                .Where(o => o.StudioId == null || o.StudioId == staff.StudioId)
                .ToList();

            _logger.LogInformation("After filtering: {Count} available orders (StudioId: {StudioId}) - Placed: {PlacedCount}, Marketplace: {MarketplaceCount}", 
                filteredOrders.Count, 
                staff.StudioId,
                orders.Items.Count(o => o.Status == ShootOrderStatus.Placed),
                orders.Items.Count(o => o.StudioId == null));

            var dtos = await EnrichOrderListDtosAsync(filteredOrders, ct);
            
            // LIMITATION: In-memory filtering after DB pagination breaks true pagination
            // Total count represents only filtered items on current page, not across all pages
            // TODO: Move filtering to repository level for accurate pagination
            if (filteredOrders.Count != orders.Items.Count)
            {
                _logger.LogWarning("Available orders pagination is approximate: DB returned {DbCount} items, filtered to {FilteredCount} items on page {Page}", 
                    orders.Items.Count, filteredOrders.Count, pageRequest.Page);
            }
            
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

            // If status is null (requesting "My Orders" - all active orders), 
            // filter out completed and cancelled orders
            var filteredOrders = orders.Items;
            int totalCount = orders.TotalCount;
            
            if (status == null)
            {
                filteredOrders = orders.Items
                    .Where(o => o.Status != ShootOrderStatus.Completed && 
                               o.Status != ShootOrderStatus.Cancelled)
                    .ToList();
                
                // LIMITATION: In-memory filtering after DB pagination breaks true pagination
                // Total count represents only filtered items on current page, not across all pages
                // TODO: Pass multiple status values to repository instead of filtering in-memory
                totalCount = filteredOrders.Count;
                
                if (filteredOrders.Count != orders.Items.Count)
                {
                    _logger.LogWarning("Photographer orders pagination is approximate: DB returned {DbCount} items, filtered to {FilteredCount} items on page {Page}", 
                        orders.Items.Count, filteredOrders.Count, pageRequest.Page);
                }
            }

            var dtos = await EnrichOrderListDtosAsync(filteredOrders, ct);
            return new PagedList<OrderListDto>(dtos, totalCount, pageRequest.Page, pageRequest.PageSize);
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

            // Load the order without tracking
            var order = await _repo.GetAggregateAsync(orderId, asNoTracking: false, ct);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            // Verify the order is available for this photographer:
            // 1. Marketplace orders (StudioId is null) - any photographer can accept
            // 2. Studio-specific orders (StudioId matches photographer's studio)
            if (order.StudioId.HasValue && order.StudioId != staff.StudioId)
            {
                _logger.LogWarning("Order {OrderId} belongs to studio {StudioId}, but photographer belongs to studio {PhotographerStudioId}", 
                    orderId, order.StudioId, staff.StudioId);
                throw new UnauthorizedAccessException("You can only accept orders from your studio or marketplace orders");
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
            
            // If this is a marketplace order, also assign it to the photographer's studio
            if (!order.StudioId.HasValue)
            {
                // This is a marketplace order, assign it to the photographer's studio
                _logger.LogInformation("Marketplace order {OrderId} being claimed by photographer from studio {StudioId}", 
                    orderId, staff.StudioId);
                order.AssignStudio(staff.StudioId);
            }
            
            // Accept the order if it's still in Placed status
            if (order.Status == ShootOrderStatus.Placed)
            {
                order.Accept();
            }

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Order {OrderId} accepted by photographer {StaffId} from studio {StudioId}", 
                orderId, staff.Id, staff.StudioId);
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

