using Logistics.Application.DTOs.Order;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        ICompanyRepository companyRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _companyRepository = companyRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdBy)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        if (await _orderRepository.GetByOrderNumberAsync(request.OrderNumber, request.CompanyId) != null)
            throw new InvalidOperationException("Número de pedido já existe");

        var order = new Order(request.CompanyId, request.OrderNumber, request.Type, request.Source);

        if (request.CustomerId.HasValue)
            order.SetCustomer(request.CustomerId.Value);

        if (request.SupplierId.HasValue)
            order.SetSupplier(request.SupplierId.Value);

        order.SetDates(request.ExpectedDate);
        order.SetPriority(request.Priority);
        order.SetShippingInfo(request.ShippingAddress, request.SpecialInstructions, request.IsBOPIS);

        decimal totalQty = 0;
        decimal totalValue = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Produto {itemRequest.ProductId} não encontrado");

            var item = new OrderItem(
                order.Id,
                itemRequest.ProductId,
                itemRequest.SKU,
                itemRequest.QuantityOrdered,
                itemRequest.UnitPrice
            );

            if (!string.IsNullOrEmpty(itemRequest.RequiredLotNumber) || itemRequest.RequiredShipDate.HasValue)
            {
                item.SetRequirements(itemRequest.RequiredLotNumber, itemRequest.RequiredShipDate);
            }

            order.AddItem(item);
            totalQty += itemRequest.QuantityOrdered;
            totalValue += itemRequest.QuantityOrdered * itemRequest.UnitPrice;
        }

        order.UpdateTotals(totalQty, totalValue);

        await _orderRepository.AddAsync(order);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(order.Id);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new KeyNotFoundException("Pedido não encontrado");
        return MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var orders = await _orderRepository.GetByCompanyIdAsync(companyId);
        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CompanyId,
            order.OrderNumber,
            order.Type,
            order.Source,
            order.CustomerId,
            order.SupplierId,
            order.OrderDate,
            order.ExpectedDate,
            order.Priority,
            order.Status,
            order.TotalQuantity,
            order.TotalValue,
            order.ShippingAddress,
            order.IsBOPIS,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.SKU,
                i.QuantityOrdered,
                i.QuantityAllocated,
                i.QuantityPicked,
                i.QuantityShipped,
                i.UnitPrice
            )).ToList(),
            order.CreatedAt
        );
    }
}
