using Reamp.Application.Orders.Dtos;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Orders.Enums;

namespace Reamp.Application.Orders.Services
{
    public interface IShootOrderAppService
    {
        Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderDto dto, Guid currentUserId, CancellationToken ct = default);
        Task<IPagedList<OrderListDto>> GetListAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default);
        Task<OrderDetailDto?> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
        
        Task AddTaskAsync(Guid orderId, AddTaskDto dto, Guid currentUserId, CancellationToken ct = default);
        Task RemoveTaskAsync(Guid orderId, Guid taskId, Guid currentUserId, CancellationToken ct = default);
        
        Task AcceptAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task ScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task StartAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task MarkAwaitingConfirmationAsync(Guid orderId, CancellationToken ct = default);
        Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task CancelAsync(Guid orderId, Guid currentUserId, string? reason = null, CancellationToken ct = default);
        
        // Photographer Assignment
        Task AssignPhotographerAsync(Guid orderId, AssignPhotographerDto dto, Guid currentUserId, CancellationToken ct = default);
        Task UnassignPhotographerAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task<IReadOnlyList<StaffSummaryDto>> GetAvailablePhotographersAsync(Guid orderId, CancellationToken ct = default);
        
        // Scheduling
        Task SetScheduleAsync(Guid orderId, SetScheduleDto dto, Guid currentUserId, CancellationToken ct = default);
        Task ClearScheduleAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        
        // Advanced Query
        Task<IPagedList<OrderListDto>> GetFilteredListAsync(OrderFilterDto filter, PageRequest pageRequest, Guid? currentUserId, CancellationToken ct = default);
        
        // Photographer-specific operations
        Task<IPagedList<OrderListDto>> GetAvailableOrdersAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default);
        Task<IPagedList<OrderListDto>> GetPhotographerOrdersAsync(PageRequest pageRequest, Guid currentUserId, ShootOrderStatus? status = null, CancellationToken ct = default);
        Task AcceptOrderAsPhotographerAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
    }
}



