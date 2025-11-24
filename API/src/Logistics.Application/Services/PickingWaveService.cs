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
    private readonly IStorageLocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PickingWaveService(
        IPickingWaveRepository repository,
        IWarehouseRepository warehouseRepository,
        IOrderRepository orderRepository,
        IStorageLocationRepository locationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _orderRepository = orderRepository;
        _locationRepository = locationRepository;
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

        // Carregar warehouse se necessário
        if (wave.Warehouse == null)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(wave.WarehouseId);
            if (warehouse == null) throw new KeyNotFoundException("Warehouse não encontrado");
        }

        // Buscar todas as Orders do tipo Outbound do warehouse para criar PickingTasks
        var orders = await _orderRepository.GetByCompanyIdAsync(wave.Warehouse?.CompanyId ?? Guid.Empty);
        var outboundOrders = orders.Where(o => o.Type == Domain.Enums.OrderType.Outbound && o.Status == Domain.Enums.OrderStatus.Confirmed).Take(5).ToList();

        // Buscar uma location padrão para as PickingLines
        var locations = await _locationRepository.GetByWarehouseIdAsync(wave.WarehouseId);
        var defaultLocation = locations.FirstOrDefault();

        if (defaultLocation != null)
        {
            int taskCounter = 1;
            int totalLines = 0;

            foreach (var order in outboundOrders)
            {
                // Criar PickingTask para cada Order
                var pickingTask = new PickingTask(
                    $"PICK-{wave.WaveNumber}-{taskCounter:D3}",
                    wave.Id,
                    order.Id
                );

                // Criar PickingLines para cada OrderItem
                if (order.Items.Any())
                {
                    foreach (var orderItem in order.Items)
                    {
                        var pickingLine = new PickingLine(
                            pickingTask.Id,
                            orderItem.Id,
                            orderItem.ProductId,
                            defaultLocation.Id,
                            orderItem.QuantityOrdered
                        );
                        pickingTask.Lines.Add(pickingLine);
                        totalLines++;
                    }
                }

                wave.Tasks.Add(pickingTask);
                taskCounter++;
            }

            wave.UpdateTotals(outboundOrders.Count, totalLines);
        }

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
