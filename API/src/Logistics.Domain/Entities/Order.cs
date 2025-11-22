using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Order
{
    private Order() { } // EF Core

    public Order(Guid companyId, string orderNumber, OrderType type, OrderSource source)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Número do pedido não pode ser vazio");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        OrderNumber = orderNumber;
        Type = type;
        Source = source;
        OrderDate = DateTime.UtcNow;
        Priority = OrderPriority.Normal;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public OrderType Type { get; private set; }
    public OrderSource Source { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public OrderPriority Priority { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? SpecialInstructions { get; private set; }
    public bool IsBOPIS { get; private set; } // Buy Online Pickup In Store
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Customer? Customer { get; private set; }
    public Supplier? Supplier { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    public void SetCustomer(Guid customerId)
    {
        CustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSupplier(Guid supplierId)
    {
        SupplierId = supplierId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDates(DateTime? expectedDate)
    {
        ExpectedDate = expectedDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPriority(OrderPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingInfo(string? shippingAddress, string? specialInstructions, bool isBOPIS)
    {
        ShippingAddress = shippingAddress;
        SpecialInstructions = specialInstructions;
        IsBOPIS = isBOPIS;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotals(decimal totalQuantity, decimal totalValue)
    {
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }
}
