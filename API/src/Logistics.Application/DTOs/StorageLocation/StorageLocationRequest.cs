namespace Logistics.Application.DTOs.StorageLocation;

public class StorageLocationRequest
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
}
