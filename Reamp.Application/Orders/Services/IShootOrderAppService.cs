using Reamp.Application.Orders.Dtos;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Orders.Services
{
    public interface IShootOrderAppService
    {
        Task<OrderDetailDto> PlaceOrderAsync(PlaceOrderDto dto, Guid currentUserId, CancellationToken ct = default);
        Task<IPagedList<OrderListDto>> GetListAsync(PageRequest pageRequest, Guid currentUserId, CancellationToken ct = default);
        Task<OrderDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        
        Task AddTaskAsync(Guid orderId, AddTaskDto dto, CancellationToken ct = default);
        Task RemoveTaskAsync(Guid orderId, Guid taskId, CancellationToken ct = default);
        
        Task AcceptAsync(Guid orderId, CancellationToken ct = default);
        Task ScheduleAsync(Guid orderId, CancellationToken ct = default);
        Task StartAsync(Guid orderId, CancellationToken ct = default);
        Task CompleteAsync(Guid orderId, CancellationToken ct = default);
        Task CancelAsync(Guid orderId, string? reason = null, CancellationToken ct = default);
    }
}



