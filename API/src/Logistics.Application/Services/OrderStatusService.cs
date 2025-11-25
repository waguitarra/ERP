using Logistics.Application.DTOs.OrderStatus;
using Logistics.Application.Interfaces;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IOrderStatusRepository _repository;

    public OrderStatusService(IOrderStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderStatusResponse>> GetAllAsync(string language = "pt")
    {
        var statuses = await _repository.GetAllActiveAsync();
        return statuses.Select(s => MapToResponse(s, language));
    }

    public async Task<OrderStatusResponse?> GetByIdAsync(int id, string language = "pt")
    {
        var statuses = await _repository.FindAsync(x => x.Id == id);
        var status = statuses.FirstOrDefault();
        return status == null ? null : MapToResponse(status, language);
    }

    public async Task<OrderStatusResponse?> GetByCodeAsync(string code, string language = "pt")
    {
        var status = await _repository.GetByCodeAsync(code);
        return status == null ? null : MapToResponse(status, language);
    }

    private static OrderStatusResponse MapToResponse(Domain.Entities.OrderStatusConfig status, string language)
    {
        var name = language.ToLower() switch
        {
            "en" => status.NameEN,
            "es" => status.NameES,
            _ => status.NamePT
        };

        var description = language.ToLower() switch
        {
            "en" => status.DescriptionEN,
            "es" => status.DescriptionES,
            _ => status.DescriptionPT
        };

        return new OrderStatusResponse
        {
            Id = status.Id,
            Code = status.Code,
            Name = name,
            Description = description,
            ColorHex = status.ColorHex,
            SortOrder = status.SortOrder
        };
    }
}
