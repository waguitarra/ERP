using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Lot;

public record LotResponse(
    Guid Id,
    Guid CompanyId,
    string LotNumber,
    Guid ProductId,
    string ProductName,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    decimal QuantityReceived,
    decimal QuantityAvailable,
    LotStatus Status,
    Guid? SupplierId,
    DateTime CreatedAt
);
