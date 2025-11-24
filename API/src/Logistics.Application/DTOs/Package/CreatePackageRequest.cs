using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Package;

public class CreatePackageRequest
{
    public Guid PackingTaskId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public PackageType Type { get; set; }
    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
}
