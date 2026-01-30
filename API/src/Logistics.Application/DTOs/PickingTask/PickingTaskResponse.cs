namespace Logistics.Application.DTOs.PickingTask;

public class PickingTaskResponse
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; } = string.Empty;
    public Guid PickingWaveId { get; set; }
    public string? WaveNumber { get; set; }
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalLines { get; set; }
    public int CompletedLines { get; set; }
    public decimal TotalQuantityToPick { get; set; }
    public decimal TotalQuantityPicked { get; set; }
    public List<PickingLineResponse> Lines { get; set; } = new();
}

public class PickingLineResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public Guid? LotId { get; set; }
    public string? LotNumber { get; set; }
    public string? SerialNumber { get; set; }
    public decimal QuantityToPick { get; set; }
    public decimal QuantityPicked { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? PickedBy { get; set; }
    public DateTime? PickedAt { get; set; }
}
