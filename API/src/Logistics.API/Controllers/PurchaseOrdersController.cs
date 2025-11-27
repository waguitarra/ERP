using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/purchase-orders")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseOrdersController(
        IPurchaseOrderRepository repository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult> GetByCompany(Guid companyId)
    {
        var purchaseOrders = await _repository.GetByCompanyIdAsync(companyId);
        return Ok(purchaseOrders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound();
        return Ok(purchaseOrder);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
    {
        // Validações
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            return BadRequest("Empresa não encontrada");

        if (await _supplierRepository.GetByIdAsync(request.SupplierId) == null)
            return BadRequest("Fornecedor não encontrado");

        if (await _repository.GetByPurchaseOrderNumberAsync(request.PurchaseOrderNumber, request.CompanyId) != null)
            return BadRequest("Número de Purchase Order já existe");

        // Criar PurchaseOrder
        var purchaseOrder = new PurchaseOrder(request.CompanyId, request.PurchaseOrderNumber, request.SupplierId);
        
        if (request.ExpectedDate.HasValue)
            purchaseOrder.SetExpectedDate(request.ExpectedDate.Value);
        
        if (request.Priority.HasValue)
            purchaseOrder.SetPriority(request.Priority.Value);

        // Adicionar items
        decimal totalQty = 0;
        decimal totalValue = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
            if (product == null)
                return BadRequest($"Produto {itemRequest.ProductId} não encontrado");

            var item = new PurchaseOrderItem(
                itemRequest.ProductId,
                itemRequest.SKU,
                itemRequest.QuantityOrdered,
                itemRequest.UnitPrice
            );

            purchaseOrder.AddItem(item);
            totalQty += itemRequest.QuantityOrdered;
            totalValue += itemRequest.QuantityOrdered * itemRequest.UnitPrice;
        }

        purchaseOrder.UpdateTotals(totalQty, totalValue);

        await _repository.AddAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = purchaseOrder.Id }, purchaseOrder);
    }

    [HttpPost("{id}/purchase-details")]
    public async Task<ActionResult> SetPurchaseDetails(Guid id, [FromBody] PurchaseOrderSetDetailsRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound();

        purchaseOrder.SetPurchaseDetails(request.UnitCost, request.TaxPercentage, request.DesiredMarginPercentage);
        
        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(purchaseOrder);
    }

    [HttpPost("{id}/packaging-hierarchy")]
    public async Task<ActionResult> SetPackagingHierarchy(Guid id, [FromBody] PurchaseOrderPackagingRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound();

        try
        {
            purchaseOrder.SetPackagingHierarchy(request.ExpectedParcels, request.CartonsPerParcel, request.UnitsPerCarton);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(purchaseOrder);
    }

    [HttpPost("{id}/set-international")]
    public async Task<ActionResult> SetAsInternational(Guid id, [FromBody] PurchaseOrderInternationalRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound();

        purchaseOrder.SetAsInternational(request.OriginCountry, request.PortOfEntry, request.ContainerNumber, request.Incoterm);

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(purchaseOrder);
    }

    [HttpPost("{id}/set-logistics")]
    public async Task<ActionResult> SetLogistics(Guid id, [FromBody] PurchaseOrderLogisticsRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound();

        purchaseOrder.SetLogistics(request.DestinationWarehouseId, request.VehicleId, request.DriverId, request.DockDoorNumber);

        if (request.ShippingDistance != null || request.ShippingCost.HasValue)
            purchaseOrder.SetShippingDetails(request.ShippingDistance, request.ShippingCost ?? 0);

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(purchaseOrder);
    }
}

public record CreatePurchaseOrderRequest(
    Guid CompanyId,
    string PurchaseOrderNumber,
    Guid SupplierId,
    DateTime? ExpectedDate,
    OrderPriority? Priority,
    List<PurchaseOrderItemRequest> Items
);

public record PurchaseOrderItemRequest(
    Guid ProductId,
    string SKU,
    decimal QuantityOrdered,
    decimal UnitPrice
);

public record PurchaseOrderSetDetailsRequest(
    decimal UnitCost,
    decimal TaxPercentage,
    decimal DesiredMarginPercentage
);

public record PurchaseOrderPackagingRequest(
    int ExpectedParcels,
    int CartonsPerParcel,
    int UnitsPerCarton
);

public record PurchaseOrderInternationalRequest(
    string OriginCountry,
    string PortOfEntry,
    string ContainerNumber,
    string Incoterm
);

public record PurchaseOrderLogisticsRequest(
    Guid? DestinationWarehouseId,
    Guid? VehicleId,
    Guid? DriverId,
    string? DockDoorNumber,
    string? ShippingDistance,
    decimal? ShippingCost
);
