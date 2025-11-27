using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.PurchaseOrder;
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
    public async Task<ActionResult<ApiResponse<IEnumerable<PurchaseOrderResponse>>>> GetByCompany(Guid companyId)
    {
        var purchaseOrders = await _repository.GetByCompanyIdAsync(companyId);
        var responses = new List<PurchaseOrderResponse>();
        foreach (var po in purchaseOrders)
        {
            responses.Add(await MapToResponse(po));
        }
        return Ok(ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(responses));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> GetById(Guid id)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));
        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> Create([FromBody] CreatePurchaseOrderRequest request)
    {
        // Validações
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            return BadRequest(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Empresa não encontrada"));

        if (await _supplierRepository.GetByIdAsync(request.SupplierId) == null)
            return BadRequest(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Fornecedor não encontrado"));

        if (await _repository.GetByPurchaseOrderNumberAsync(request.PurchaseOrderNumber, request.CompanyId) != null)
            return BadRequest(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Número de Purchase Order já existe"));

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
                return BadRequest(ApiResponse<PurchaseOrderResponse>.ErrorResponse($"Produto {itemRequest.ProductId} não encontrado"));

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

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Purchase order criado com sucesso"));
    }

    [HttpPost("{id}/purchase-details")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> SetPurchaseDetails(Guid id, [FromBody] PurchaseOrderSetDetailsRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));

        purchaseOrder.SetPurchaseDetails(request.UnitCost, request.TaxPercentage, request.DesiredMarginPercentage);
        
        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Detalhes de compra atualizados"));
    }

    [HttpPost("{id}/packaging-hierarchy")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> SetPackagingHierarchy(Guid id, [FromBody] PurchaseOrderPackagingRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));

        try
        {
            purchaseOrder.SetPackagingHierarchy(request.ExpectedParcels, request.CartonsPerParcel, request.UnitsPerCarton);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<PurchaseOrderResponse>.ErrorResponse(ex.Message));
        }

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Hierarquia de embalagem definida"));
    }

    [HttpPost("{id}/set-international")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> SetAsInternational(Guid id, [FromBody] PurchaseOrderInternationalRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));

        purchaseOrder.SetAsInternational(request.OriginCountry, request.PortOfEntry, request.ContainerNumber, request.Incoterm);

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Configuração internacional definida"));
    }

    [HttpPost("{id}/set-logistics")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> SetLogistics(Guid id, [FromBody] PurchaseOrderLogisticsRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));

        purchaseOrder.SetLogistics(request.DestinationWarehouseId, request.VehicleId, request.DriverId, request.DockDoorNumber);

        if (request.ShippingDistance != null || request.ShippingCost.HasValue)
            purchaseOrder.SetShippingDetails(request.ShippingDistance, request.ShippingCost ?? 0);

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Configuração logística definida"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderResponse>>> Update(Guid id, [FromBody] UpdatePurchaseOrderRequest request)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase order não encontrado"));

        // Atualizar campos básicos
        if (request.ExpectedDate.HasValue)
            purchaseOrder.SetExpectedDate(request.ExpectedDate.Value);
        
        if (request.Priority.HasValue)
            purchaseOrder.SetPriority(request.Priority.Value);

        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapToResponse(purchaseOrder), "Purchase order atualizado com sucesso"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var purchaseOrder = await _repository.GetByIdAsync(id);
        if (purchaseOrder == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Purchase order não encontrado"));

        purchaseOrder.SetStatus(OrderStatus.Cancelled);
        await _repository.UpdateAsync(purchaseOrder);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Purchase order cancelado com sucesso"));
    }

    private async Task<PurchaseOrderResponse> MapToResponse(PurchaseOrder po)
    {
        var supplier = await _supplierRepository.GetByIdAsync(po.SupplierId);
        var response = new PurchaseOrderResponse();
        response.Id = po.Id;
        response.CompanyId = po.CompanyId;
        response.PurchaseOrderNumber = po.PurchaseOrderNumber;
        response.SupplierId = po.SupplierId;
        response.SupplierName = supplier?.Name ?? "Fornecedor não encontrado";
        response.OrderDate = po.OrderDate;
        response.ExpectedDate = po.ExpectedDate;
        response.Priority = po.Priority;
        response.Status = po.Status;
        response.TotalQuantity = po.TotalQuantity;
        response.TotalValue = po.TotalValue;
        response.Items = po.Items.Select(i => new PurchaseOrderItemResponse(
            i.Id,
            i.ProductId,
            i.SKU,
            i.QuantityOrdered,
            i.QuantityReceived,
            i.UnitPrice
        )).ToList();
        return response;
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

public record UpdatePurchaseOrderRequest(
    DateTime? ExpectedDate,
    OrderPriority? Priority
);
