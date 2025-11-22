using Logistics.Application.DTOs.SerialNumber;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class SerialNumberService : ISerialNumberService
{
    private readonly ISerialNumberRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SerialNumberService(
        ISerialNumberRepository repository,
        IProductRepository productRepository,
        ILotRepository lotRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _productRepository = productRepository;
        _lotRepository = lotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SerialNumberResponse> CreateAsync(CreateSerialNumberRequest request)
    {
        if (await _productRepository.GetByIdAsync(request.ProductId) == null)
            throw new KeyNotFoundException("Produto não encontrado");

        if (await _lotRepository.GetByIdAsync(request.LotId) == null)
            throw new KeyNotFoundException("Lote não encontrado");

        if (await _repository.GetBySerialAsync(request.Serial) != null)
            throw new InvalidOperationException("Número de série já existe");

        var serialNumber = new SerialNumber(
            request.Serial,
            request.ProductId,
            request.LotId
        );

        await _repository.AddAsync(serialNumber);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(serialNumber.Id);
    }

    public async Task<SerialNumberResponse> GetByIdAsync(Guid id)
    {
        var serialNumber = await _repository.GetByIdAsync(id);
        if (serialNumber == null) throw new KeyNotFoundException("Número de série não encontrado");
        return MapToResponse(serialNumber);
    }

    public async Task<SerialNumberResponse> GetBySerialAsync(string serial)
    {
        var serialNumber = await _repository.GetBySerialAsync(serial);
        if (serialNumber == null) throw new KeyNotFoundException("Número de série não encontrado");
        return MapToResponse(serialNumber);
    }

    public async Task<IEnumerable<SerialNumberResponse>> GetByProductIdAsync(Guid productId)
    {
        var serialNumbers = await _repository.GetByProductIdAsync(productId);
        return serialNumbers.Select(MapToResponse);
    }

    public async Task<IEnumerable<SerialNumberResponse>> GetByLotIdAsync(Guid lotId)
    {
        var serialNumbers = await _repository.GetByLotIdAsync(lotId);
        return serialNumbers.Select(MapToResponse);
    }

    public async Task ReceiveAsync(Guid id, Guid locationId)
    {
        var serialNumber = await _repository.GetByIdAsync(id);
        if (serialNumber == null) throw new KeyNotFoundException("Número de série não encontrado");

        serialNumber.Receive(DateTime.UtcNow, locationId);
        await _repository.UpdateAsync(serialNumber);
        await _unitOfWork.CommitAsync();
    }

    public async Task ShipAsync(Guid id)
    {
        var serialNumber = await _repository.GetByIdAsync(id);
        if (serialNumber == null) throw new KeyNotFoundException("Número de série não encontrado");

        serialNumber.Ship(DateTime.UtcNow);
        await _repository.UpdateAsync(serialNumber);
        await _unitOfWork.CommitAsync();
    }

    private static SerialNumberResponse MapToResponse(SerialNumber serialNumber)
    {
        return new SerialNumberResponse(
            serialNumber.Id,
            serialNumber.Serial,
            serialNumber.ProductId,
            serialNumber.Product?.Name ?? "",
            serialNumber.LotId,
            serialNumber.Lot?.LotNumber ?? "",
            serialNumber.Status,
            serialNumber.CurrentLocationId,
            serialNumber.ReceivedDate,
            serialNumber.ShippedDate,
            serialNumber.CreatedAt
        );
    }
}
