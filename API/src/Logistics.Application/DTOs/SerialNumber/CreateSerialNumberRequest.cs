namespace Logistics.Application.DTOs.SerialNumber;

public record CreateSerialNumberRequest(
    string Serial,
    Guid ProductId,
    Guid LotId
);
