using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/sales-orders")]
[Authorize]
public class SalesOrdersController : ControllerBase
{
    private readonly ISalesOrderRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SalesOrdersController(
        ISalesOrderRepository repository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult> GetByCompany(Guid companyId)
    {
        var salesOrders = await _repository.GetByCompanyIdAsync(companyId);
        var response = salesOrders.Select(so => new SalesOrderResponse
        {
            Id = so.Id,
            CompanyId = so.CompanyId,
            SalesOrderNumber = so.SalesOrderNumber,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer?.Name,
            OrderDate = so.OrderDate,
            ExpectedDate = so.ExpectedDate,
            Priority = so.Priority.ToString(),
            Status = so.Status.ToString(),
            TotalQuantity = so.TotalQuantity,
            TotalValue = so.TotalValue,
            ShippingAddress = so.ShippingAddress,
            IsBOPIS = so.IsBOPIS,
            CreatedAt = so.CreatedAt,
            Items = so.Items.Select(i => new SalesOrderItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Sku = i.SKU,
                QuantityOrdered = i.QuantityOrdered,
                QuantityAllocated = i.QuantityAllocated,
                QuantityPicked = i.QuantityPicked,
                QuantityShipped = i.QuantityShipped,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var salesOrder = await _repository.GetByIdAsync(id);
        if (salesOrder == null)
            return NotFound();
        return Ok(salesOrder);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSalesOrderRequest request)
    {
        // Validações
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            return BadRequest("Empresa não encontrada");

        if (await _customerRepository.GetByIdAsync(request.CustomerId) == null)
            return BadRequest("Cliente não encontrado");

        if (await _repository.GetBySalesOrderNumberAsync(request.SalesOrderNumber, request.CompanyId) != null)
            return BadRequest("Número de Sales Order já existe");

        // Criar SalesOrder
        var salesOrder = new SalesOrder(request.CompanyId, request.SalesOrderNumber, request.CustomerId);
        
        if (request.ExpectedDate.HasValue)
            salesOrder.SetExpectedDate(request.ExpectedDate.Value);
        
        if (request.Priority.HasValue)
            salesOrder.SetPriority(request.Priority.Value);

        if (!string.IsNullOrWhiteSpace(request.ShippingAddress))
            salesOrder.SetShippingAddress(request.ShippingAddress, null, null, null, null);

        // Adicionar items
        decimal totalQty = 0;
        decimal totalValue = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
            if (product == null)
                return BadRequest($"Produto {itemRequest.ProductId} não encontrado");

            var item = new SalesOrderItem(
                itemRequest.ProductId,
                itemRequest.SKU,
                itemRequest.QuantityOrdered,
                itemRequest.UnitPrice
            );

            salesOrder.AddItem(item);
            totalQty += itemRequest.QuantityOrdered;
            totalValue += itemRequest.QuantityOrdered * itemRequest.UnitPrice;
        }

        salesOrder.UpdateTotals(totalQty, totalValue);

        await _repository.AddAsync(salesOrder);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = salesOrder.Id }, salesOrder);
    }

    [HttpPost("{id}/packaging-hierarchy")]
    public async Task<ActionResult> SetPackagingHierarchy(Guid id, [FromBody] SalesOrderPackagingRequest request)
    {
        var salesOrder = await _repository.GetByIdAsync(id);
        if (salesOrder == null)
            return NotFound();

        try
        {
            salesOrder.SetPackagingHierarchy(request.ExpectedParcels, request.CartonsPerParcel, request.UnitsPerCarton);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        await _repository.UpdateAsync(salesOrder);
        await _unitOfWork.CommitAsync();

        return Ok(salesOrder);
    }

    [HttpPost("{id}/mark-shipped")]
    public async Task<ActionResult> MarkAsShipped(Guid id)
    {
        var salesOrder = await _repository.GetByIdAsync(id);
        if (salesOrder == null)
            return NotFound();

        salesOrder.MarkAsShipped(DateTime.UtcNow);

        await _repository.UpdateAsync(salesOrder);
        await _unitOfWork.CommitAsync();

        return Ok(salesOrder);
    }

    [HttpPost("{id}/mark-delivered")]
    public async Task<ActionResult> MarkAsDelivered(Guid id)
    {
        var salesOrder = await _repository.GetByIdAsync(id);
        if (salesOrder == null)
            return NotFound();

        salesOrder.MarkAsDelivered(DateTime.UtcNow);

        await _repository.UpdateAsync(salesOrder);
        await _unitOfWork.CommitAsync();

        return Ok(salesOrder);
    }
}

public record CreateSalesOrderRequest(
    Guid CompanyId,
    string SalesOrderNumber,
    Guid CustomerId,
    DateTime? ExpectedDate,
    OrderPriority? Priority,
    string? ShippingAddress,
    List<SalesOrderItemRequest> Items
);

public record SalesOrderItemRequest(
    Guid ProductId,
    string SKU,
    decimal QuantityOrdered,
    decimal UnitPrice
);

public record SalesOrderPackagingRequest(
    int ExpectedParcels,
    int CartonsPerParcel,
    int UnitsPerCarton
);

public class SalesOrderResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string SalesOrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public string? ShippingAddress { get; set; }
    public bool IsBOPIS { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SalesOrderItemResponse> Items { get; set; } = new();
}

public class SalesOrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityAllocated { get; set; }
    public decimal QuantityPicked { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal UnitPrice { get; set; }
}
