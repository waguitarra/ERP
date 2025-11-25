using Logistics.Application.DTOs.Order;

namespace Logistics.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdBy);
    Task<OrderResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request);
}
