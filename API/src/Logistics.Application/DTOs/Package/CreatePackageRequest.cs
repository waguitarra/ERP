using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Package;

public record CreatePackageRequest(
    Guid PackingTaskId,
    string TrackingNumber,
    PackageType Type,
    decimal Weight,
    decimal Length,
    decimal Width,
    decimal Height
);
