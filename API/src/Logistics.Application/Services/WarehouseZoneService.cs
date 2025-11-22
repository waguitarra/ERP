using Logistics.Application.DTOs.WarehouseZone;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class WarehouseZoneService : IWarehouseZoneService
{
    private readonly IWarehouseZoneRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseZoneService(
        IWarehouseZoneRepository repository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseZoneResponse> CreateAsync(CreateWarehouseZoneRequest request)
    {
        if (await _warehouseRepository.GetByIdAsync(request.WarehouseId) == null)
            throw new KeyNotFoundException("Armazém não encontrado");

        var zone = new WarehouseZone(request.WarehouseId, request.ZoneName, request.Type);
        zone.Update(request.ZoneName, request.Type, request.Temperature, request.Humidity, request.TotalCapacity);

        await _repository.AddAsync(zone);
        await _unitOfWork.CommitAsync();

        return MapToResponse(zone);
    }

    public async Task<WarehouseZoneResponse> GetByIdAsync(Guid id)
    {
        var zone = await _repository.GetByIdAsync(id);
        if (zone == null) throw new KeyNotFoundException("Zona não encontrada");
        return MapToResponse(zone);
    }

    public async Task<IEnumerable<WarehouseZoneResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var zones = await _repository.GetByWarehouseIdAsync(warehouseId);
        return zones.Select(MapToResponse);
    }

    public async Task<IEnumerable<WarehouseZoneResponse>> GetAllAsync()
    {
        var zones = await _repository.GetAllAsync();
        return zones.Select(MapToResponse);
    }

    private static WarehouseZoneResponse MapToResponse(WarehouseZone zone)
    {
        return new WarehouseZoneResponse(
            zone.Id,
            zone.WarehouseId,
            zone.ZoneName,
            zone.Type,
            zone.Temperature,
            zone.Humidity,
            zone.TotalCapacity,
            zone.UsedCapacity,
            zone.IsActive,
            zone.CreatedAt
        );
    }
}
