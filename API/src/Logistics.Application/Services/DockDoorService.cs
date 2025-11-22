using Logistics.Application.DTOs.DockDoor;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class DockDoorService : IDockDoorService
{
    private readonly IDockDoorRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DockDoorService(
        IDockDoorRepository repository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DockDoorResponse> CreateAsync(CreateDockDoorRequest request)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");

        var dockDoor = new DockDoor(request.WarehouseId, request.DoorNumber, request.Type);

        await _repository.AddAsync(dockDoor);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(dockDoor.Id);
    }

    public async Task<DockDoorResponse> GetByIdAsync(Guid id)
    {
        var dockDoor = await _repository.GetByIdAsync(id);
        if (dockDoor == null) throw new KeyNotFoundException("Doca não encontrada");
        return MapToResponse(dockDoor);
    }

    public async Task<IEnumerable<DockDoorResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var dockDoors = await _repository.GetByWarehouseIdAsync(warehouseId);
        return dockDoors.Select(MapToResponse);
    }

    public async Task<IEnumerable<DockDoorResponse>> GetAvailableAsync(Guid warehouseId)
    {
        var dockDoors = await _repository.GetAvailableAsync(warehouseId);
        return dockDoors.Select(MapToResponse);
    }

    public async Task<IEnumerable<DockDoorResponse>> GetAllAsync()
    {
        var dockDoors = await _repository.GetAllAsync();
        return dockDoors.Select(MapToResponse);
    }

    private static DockDoorResponse MapToResponse(DockDoor dockDoor)
    {
        return new DockDoorResponse(
            dockDoor.Id,
            dockDoor.WarehouseId,
            dockDoor.Warehouse?.Name ?? "",
            dockDoor.DoorNumber,
            dockDoor.Type,
            dockDoor.Status,
            dockDoor.IsActive,
            dockDoor.CreatedAt
        );
    }
}
