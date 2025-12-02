using Microsoft.Extensions.Logging;
using Reamp.Application.Orders.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Repositories;

namespace Reamp.Application.Orders.Services
{
    public sealed class ShootOrderAppService : IShootOrderAppService
    {
        private readonly IShootOrderRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ShootOrderAppService> _logger;

        public ShootOrderAppService(
            IShootOrderRepository repo,
            IUnitOfWork uow,
            ILogger<ShootOrderAppService> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Placing order for listing {ListingId} by user {UserId}", dto.ListingId, currentUserId);

            // Create order
            var order = ShootOrder.Place(
                agencyId: dto.AgencyId,
                studioId: dto.StudioId,
                listingId: dto.ListingId,
                createdBy: currentUserId,
                currency: dto.Currency ?? "AUD"
            );

            await _repo.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully placed order {OrderId}", order.Id);

            return MapToDetailDto(order);
        }

        public async Task<IPagedList<OrderListDto>> GetListAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogDebug("Getting order list for user {UserId}", currentUserId);

            // Get orders for current user
            var orders = await _repo.ListAsync(pageRequest, createdBy: currentUserId, ct: ct);

            // Map to DTOs
            var dtos = orders.Items.Select(MapToListDto).ToList();

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

            return MapToDetailDto(order);
        }

        public async Task AddTaskAsync(Guid orderId, AddTaskDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding task {TaskType} to order {OrderId} by user {UserId}", dto.Type, orderId, currentUserId);

            // Load order
            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

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
                throw new InvalidOperationException($"Order {orderId} not found");

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
            _logger.LogInformation("Accepting order {OrderId} by user {UserId}", orderId, currentUserId);

            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            order.Accept();

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully accepted order {OrderId}", orderId);
        }

        public async Task ScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Scheduling order {OrderId} by user {UserId}", orderId, currentUserId);

            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            order.MarkScheduled();

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully scheduled order {OrderId}", orderId);
        }

        public async Task StartAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Starting order {OrderId} by user {UserId}", orderId, currentUserId);

            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            order.Start();

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully started order {OrderId}", orderId);
        }

        public async Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default)
        {
            _logger.LogInformation("Completing order {OrderId} by user {UserId}", orderId, currentUserId);

            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            order.Complete();

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully completed order {OrderId}", orderId);
        }

        public async Task CancelAsync(Guid orderId, Guid currentUserId, string? reason = null, CancellationToken ct = default)
        {
            _logger.LogInformation("Cancelling order {OrderId} by user {UserId} with reason: {Reason}", orderId, currentUserId, reason ?? "N/A");

            var order = await _repo.GetAggregateAsync(orderId, ct);
            if (order == null)
                throw new InvalidOperationException($"Order {orderId} not found");

            // Verify ownership
            if (order.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to modify order {OrderId} owned by {OwnerId}", 
                    currentUserId, orderId, order.CreatedBy);
                throw new UnauthorizedAccessException("You do not have permission to modify this order");
            }

            order.Cancel(reason ?? string.Empty);

            await _repo.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully cancelled order {OrderId}", orderId);
        }

        // Map entity to list DTO
        private static OrderListDto MapToListDto(ShootOrder order)
        {
            return new OrderListDto
            {
                Id = order.Id,
                AgencyId = order.AgencyId,
                StudioId = order.StudioId,
                ListingId = order.ListingId,
                Currency = order.Currency,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAtUtc = order.CreatedAtUtc,
                TaskCount = order.Tasks.Count
            };
        }

        // Map entity to detail DTO
        private static OrderDetailDto MapToDetailDto(ShootOrder order)
        {
            return new OrderDetailDto
            {
                Id = order.Id,
                AgencyId = order.AgencyId,
                StudioId = order.StudioId,
                ListingId = order.ListingId,
                Currency = order.Currency,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedBy = order.CreatedBy,
                CreatedAtUtc = order.CreatedAtUtc,
                Tasks = order.Tasks.Select(t => new TaskDetailDto
                {
                    Id = t.Id,
                    Type = t.Type,
                    Status = t.Status,
                    Price = t.Price,
                    Notes = t.Notes,
                    ScheduledStartUtc = t.ScheduledStartUtc,
                    ScheduledEndUtc = t.ScheduledEndUtc,
                    AssigneeUserId = t.AssigneeUserId
                }).ToList()
            };
        }

        // Helper class for paged list mapping
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

