using Logistics.Application.DTOs.OrderStatus;

namespace Logistics.Application.Interfaces;

public interface IOrderStatusService
{
    Task<IEnumerable<OrderStatusResponse>> GetAllAsync(string language = "pt");
    Task<OrderStatusResponse?> GetByIdAsync(int id, string language = "pt");
    Task<OrderStatusResponse?> GetByCodeAsync(string code, string language = "pt");
}
