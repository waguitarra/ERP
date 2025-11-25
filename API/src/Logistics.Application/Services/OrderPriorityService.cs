using Logistics.Application.DTOs.OrderPriority;
using Logistics.Application.Interfaces;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class OrderPriorityService : IOrderPriorityService
{
    private readonly IOrderPriorityRepository _repository;

    public OrderPriorityService(IOrderPriorityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderPriorityResponse>> GetAllAsync(string language = "pt")
    {
        var priorities = await _repository.GetAllActiveAsync();
        return priorities.Select(p => MapToResponse(p, language));
    }

    public async Task<OrderPriorityResponse?> GetByIdAsync(int id, string language = "pt")
    {
        var priorities = await _repository.FindAsync(x => x.Id == id);
        var priority = priorities.FirstOrDefault();
        return priority == null ? null : MapToResponse(priority, language);
    }

    public async Task<OrderPriorityResponse?> GetByCodeAsync(string code, string language = "pt")
    {
        var priority = await _repository.GetByCodeAsync(code);
        return priority == null ? null : MapToResponse(priority, language);
    }

    private static OrderPriorityResponse MapToResponse(Domain.Entities.OrderPriorityConfig priority, string language)
    {
        var name = language.ToLower() switch
        {
            "en" => priority.NameEN,
            "es" => priority.NameES,
            _ => priority.NamePT
        };

        var description = language.ToLower() switch
        {
            "en" => priority.DescriptionEN,
            "es" => priority.DescriptionES,
            _ => priority.DescriptionPT
        };

        return new OrderPriorityResponse
        {
            Id = priority.Id,
            Code = priority.Code,
            Name = name,
            Description = description,
            ColorHex = priority.ColorHex,
            SortOrder = priority.SortOrder
        };
    }
}
