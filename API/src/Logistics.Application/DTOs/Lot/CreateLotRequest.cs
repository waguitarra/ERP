namespace Logistics.Application.DTOs.Lot;

public record CreateLotRequest(
    Guid CompanyId,
    string LotNumber,
    Guid ProductId,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    decimal QuantityReceived,
    Guid? SupplierId
);
