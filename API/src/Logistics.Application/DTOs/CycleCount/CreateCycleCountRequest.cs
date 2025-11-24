namespace Logistics.Application.DTOs.CycleCount;

public class CreateCycleCountRequest
{
    public string CountNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? ZoneId { get; set; }
    public Guid CountedBy { get; set; }
}
