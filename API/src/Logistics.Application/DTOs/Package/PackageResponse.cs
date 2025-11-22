using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Package;

public record PackageResponse(
    Guid Id,
    Guid PackingTaskId,
    string TrackingNumber,
    PackageType Type,
    decimal Weight,
    decimal Length,
    decimal Width,
    decimal Height,
    PackageStatus Status,
    DateTime CreatedAt
);
