using Logistics.Application.DTOs.PickingWave;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class PickingWaveService : IPickingWaveService
{
    private readonly IPickingWaveRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PickingWaveService(
        IPickingWaveRepository repository,
        IWarehouseRepository warehouseRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PickingWaveResponse> CreateAsync(CreatePickingWaveRequest request)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");

        var wave = new PickingWave(request.WaveNumber, request.WarehouseId);
        
        wave.UpdateTotals(request.OrderIds.Count, 0);

        await _repository.AddAsync(wave);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(wave.Id);
    }

    public async Task<PickingWaveResponse> GetByIdAsync(Guid id)
    {
        var wave = await _repository.GetByIdAsync(id);
        if (wave == null) throw new KeyNotFoundException("Picking Wave não encontrada");
        return MapToResponse(wave);
    }

    public async Task<IEnumerable<PickingWaveResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var waves = await _repository.GetByWarehouseIdAsync(warehouseId);
        return waves.Select(MapToResponse);
    }

    public async Task<IEnumerable<PickingWaveResponse>> GetAllAsync()
    {
        var waves = await _repository.GetAllAsync();
        return waves.Select(MapToResponse);
    }

    public async Task ReleaseAsync(Guid id)
    {
        var wave = await _repository.GetByIdAsync(id);
        if (wave == null) throw new KeyNotFoundException("Picking Wave não encontrada");

        wave.Release();
        await _unitOfWork.CommitAsync();
    }

    private static PickingWaveResponse MapToResponse(PickingWave wave)
    {
        return new PickingWaveResponse(
            wave.Id,
            wave.WaveNumber,
            wave.WarehouseId,
            wave.Warehouse?.Name ?? "",
            wave.Status,
            wave.ReleasedAt,
            wave.CompletedAt,
            wave.TotalOrders,
            wave.TotalLines,
            wave.CreatedAt
        );
    }
}
