using Logistics.Application.DTOs.OrderPriority;

namespace Logistics.Application.Interfaces;

public interface IOrderPriorityService
{
    Task<IEnumerable<OrderPriorityResponse>> GetAllAsync(string language = "pt");
    Task<OrderPriorityResponse?> GetByIdAsync(int id, string language = "pt");
    Task<OrderPriorityResponse?> GetByCodeAsync(string code, string language = "pt");
}
