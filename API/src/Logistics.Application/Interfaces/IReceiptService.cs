using Logistics.Application.DTOs.Receipt;

namespace Logistics.Application.Interfaces;

public interface IReceiptService
{
    Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request);
    Task<ReceiptResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<ReceiptResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<ReceiptResponse>> GetAllAsync();
}
