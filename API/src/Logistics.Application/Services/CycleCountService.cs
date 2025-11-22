using Logistics.Application.DTOs.CycleCount;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class CycleCountService : ICycleCountService
{
    private readonly ICycleCountRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CycleCountService(
        ICycleCountRepository repository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CycleCountResponse> CreateAsync(CreateCycleCountRequest request)
    {
        if (await _warehouseRepository.GetByIdAsync(request.WarehouseId) == null)
            throw new KeyNotFoundException("Armazém não encontrado");

        var cycleCount = new CycleCount(
            request.CountNumber,
            request.WarehouseId,
            request.ZoneId,
            request.CountedBy
        );

        await _repository.AddAsync(cycleCount);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(cycleCount.Id);
    }

    public async Task<CycleCountResponse> GetByIdAsync(Guid id)
    {
        var cycleCount = await _repository.GetByIdAsync(id);
        if (cycleCount == null) throw new KeyNotFoundException("Contagem cíclica não encontrada");
        return MapToResponse(cycleCount);
    }

    public async Task<IEnumerable<CycleCountResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var cycleCounts = await _repository.GetByWarehouseIdAsync(warehouseId);
        return cycleCounts.Select(MapToResponse);
    }

    public async Task CompleteAsync(Guid id)
    {
        var cycleCount = await _repository.GetByIdAsync(id);
        if (cycleCount == null) throw new KeyNotFoundException("Contagem cíclica não encontrada");

        cycleCount.Complete();
        await _repository.UpdateAsync(cycleCount);
        await _unitOfWork.CommitAsync();
    }

    private static CycleCountResponse MapToResponse(CycleCount cycleCount)
    {
        return new CycleCountResponse(
            cycleCount.Id,
            cycleCount.CountNumber,
            cycleCount.WarehouseId,
            cycleCount.Warehouse?.Name ?? "",
            cycleCount.ZoneId,
            cycleCount.CountDate,
            cycleCount.Status,
            cycleCount.CountedBy,
            cycleCount.CreatedAt
        );
    }
}
