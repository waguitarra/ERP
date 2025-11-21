using Logistics.Application.DTOs.Product;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");
        if (await _productRepository.SKUExistsAsync(request.SKU))
            throw new InvalidOperationException("SKU já existe");

        var product = new Product(request.CompanyId, request.Name, request.SKU, request.Barcode);
        product.Update(request.Name, request.SKU, request.Barcode, request.Description, request.Weight, request.WeightUnit);
        
        await _productRepository.AddAsync(product);
        await _unitOfWork.CommitAsync();
        return MapToResponse(product);
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Produto não encontrado");
        return MapToResponse(product);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToResponse);
    }

    public async Task<IEnumerable<ProductResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var products = await _productRepository.GetByCompanyIdAsync(companyId);
        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Produto não encontrado");
        if (await _productRepository.SKUExistsAsync(request.SKU, id))
            throw new InvalidOperationException("SKU já existe");

        product.Update(request.Name, request.SKU, request.Barcode, request.Description, request.Weight, request.WeightUnit);
        await _productRepository.UpdateAsync(product);
        await _unitOfWork.CommitAsync();
        return MapToResponse(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Produto não encontrado");
        await _productRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    private static ProductResponse MapToResponse(Product p) => new ProductResponse
    {
        Id = p.Id, CompanyId = p.CompanyId, Name = p.Name, SKU = p.SKU, Barcode = p.Barcode,
        Description = p.Description, Weight = p.Weight, WeightUnit = p.WeightUnit,
        IsActive = p.IsActive, CreatedAt = p.CreatedAt
    };
}
