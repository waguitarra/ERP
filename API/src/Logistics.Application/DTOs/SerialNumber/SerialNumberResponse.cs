using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.SerialNumber;

public record SerialNumberResponse(
    Guid Id,
    string Serial,
    Guid ProductId,
    string ProductName,
    Guid LotId,
    string LotNumber,
    SerialStatus Status,
    Guid? CurrentLocationId,
    DateTime? ReceivedDate,
    DateTime? ShippedDate,
    DateTime CreatedAt
);
