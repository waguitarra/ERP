namespace Logistics.Application.DTOs.OrderPriority;

public class OrderPriorityResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ColorHex { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
