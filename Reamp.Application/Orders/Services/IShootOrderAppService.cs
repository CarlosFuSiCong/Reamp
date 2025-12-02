using Reamp.Application.Orders.Dtos;
using Reamp.Domain.Common.Abstractions;

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
        Task CompleteAsync(Guid orderId, Guid currentUserId, CancellationToken ct = default);
        Task CancelAsync(Guid orderId, Guid currentUserId, string? reason = null, CancellationToken ct = default);
    }
}



