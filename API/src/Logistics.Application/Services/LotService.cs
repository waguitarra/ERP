using Logistics.Application.DTOs.Lot;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class LotService : ILotService
{
    private readonly ILotRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LotService(
        ILotRepository repository,
        ICompanyRepository companyRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LotResponse> CreateAsync(CreateLotRequest request)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        if (await _productRepository.GetByIdAsync(request.ProductId) == null)
            throw new KeyNotFoundException("Produto não encontrado");

        if (await _repository.GetByLotNumberAsync(request.LotNumber, request.CompanyId) != null)
            throw new InvalidOperationException("Número de lote já existe");

        var lot = new Lot(
            request.CompanyId,
            request.LotNumber,
            request.ProductId,
            request.ManufactureDate,
            request.ExpiryDate,
            request.QuantityReceived
        );

        if (request.SupplierId.HasValue)
            lot.SetSupplier(request.SupplierId.Value);

        await _repository.AddAsync(lot);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(lot.Id);
    }

    public async Task<LotResponse> GetByIdAsync(Guid id)
    {
        var lot = await _repository.GetByIdAsync(id);
        if (lot == null) throw new KeyNotFoundException("Lote não encontrado");
        return MapToResponse(lot);
    }

    public async Task<IEnumerable<LotResponse>> GetByProductIdAsync(Guid productId)
    {
        var lots = await _repository.GetByProductIdAsync(productId);
        return lots.Select(MapToResponse);
    }

    public async Task<IEnumerable<LotResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var lots = await _repository.GetByCompanyIdAsync(companyId);
        return lots.Select(MapToResponse);
    }

    public async Task<IEnumerable<LotResponse>> GetExpiringLotsAsync(Guid companyId, int daysAhead)
    {
        var beforeDate = DateTime.UtcNow.AddDays(daysAhead);
        var lots = await _repository.GetExpiringLotsAsync(companyId, beforeDate);
        return lots.Select(MapToResponse);
    }

    public async Task QuarantineLotAsync(Guid id)
    {
        var lot = await _repository.GetByIdAsync(id);
        if (lot == null) throw new KeyNotFoundException("Lote não encontrado");

        lot.QuarantineLot();
        await _repository.UpdateAsync(lot);
        await _unitOfWork.CommitAsync();
    }

    public async Task ReleaseLotAsync(Guid id)
    {
        var lot = await _repository.GetByIdAsync(id);
        if (lot == null) throw new KeyNotFoundException("Lote não encontrado");

        lot.ReleaseLot();
        await _repository.UpdateAsync(lot);
        await _unitOfWork.CommitAsync();
    }

    private static LotResponse MapToResponse(Lot lot)
    {
        return new LotResponse(
            lot.Id,
            lot.CompanyId,
            lot.LotNumber,
            lot.ProductId,
            lot.Product?.Name ?? "",
            lot.ManufactureDate,
            lot.ExpiryDate,
            lot.QuantityReceived,
            lot.QuantityAvailable,
            lot.Status,
            lot.SupplierId,
            lot.CreatedAt
        );
    }
}
