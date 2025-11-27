using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class SalesOrder
{
    private SalesOrder() { } // EF Core

    public SalesOrder(Guid companyId, string salesOrderNumber, Guid customerId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        if (string.IsNullOrWhiteSpace(salesOrderNumber))
            throw new ArgumentException("Número do pedido de venda não pode ser vazio");
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId é obrigatório para Sales Order");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        SalesOrderNumber = salesOrderNumber;
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Priority = OrderPriority.Normal;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string SalesOrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public OrderPriority Priority { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? SpecialInstructions { get; private set; }
    
    // Endereço de entrega (VENDA)
    public string? ShippingAddress { get; private set; }
    public string? ShippingZipCode { get; private set; }
    public decimal? ShippingLatitude { get; private set; }
    public decimal? ShippingLongitude { get; private set; }
    public string? ShippingCity { get; private set; }
    public string? ShippingState { get; private set; }
    public string? ShippingCountry { get; private set; }
    
    // Rastreamento (VENDA)
    public string? TrackingNumber { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }
    public DateTime? ActualDeliveryDate { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    
    // BOPIS (Buy Online Pickup In Store)
    public bool IsBOPIS { get; private set; }
    
    // Hierarquia de Embalagem (EXPEDIÇÃO) - NOVO conforme solicitado
    public int ExpectedParcels { get; private set; }
    public int PackedParcels { get; private set; }
    public int ExpectedCartons { get; private set; }
    public int UnitsPerCarton { get; private set; }
    public int CartonsPerParcel { get; private set; }
    
    // WMS
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Guid? OriginWarehouseId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public ICollection<SalesOrderItem> Items { get; private set; } = new List<SalesOrderItem>();

    // Métodos de negócio
    public void SetExpectedDate(DateTime? expectedDate)
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

    public void AddItem(SalesOrderItem item)
    {
        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotals(decimal totalQuantity, decimal totalValue)
    {
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingAddress(string address, string? zipCode, string? city, string? state, string? country)
    {
        ShippingAddress = address;
        ShippingZipCode = zipCode;
        ShippingCity = city;
        ShippingState = state;
        ShippingCountry = country;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number inválido");

        TrackingNumber = trackingNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBOPIS(bool isBOPIS)
    {
        IsBOPIS = isBOPIS;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsShipped(DateTime shippedAt)
    {
        ShippedAt = shippedAt;
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered(DateTime deliveredAt)
    {
        DeliveredAt = deliveredAt;
        ActualDeliveryDate = deliveredAt;
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    // NOVO: Hierarquia de embalagem para expedição
    public void SetPackagingHierarchy(int expectedParcels, int cartonsPerParcel, int unitsPerCarton)
    {
        if (expectedParcels <= 0 || cartonsPerParcel <= 0 || unitsPerCarton <= 0)
            throw new ArgumentException("Valores de hierarquia devem ser maiores que zero");

        ExpectedParcels = expectedParcels;
        CartonsPerParcel = cartonsPerParcel;
        UnitsPerCarton = unitsPerCarton;
        ExpectedCartons = expectedParcels * cartonsPerParcel;
        
        var calculatedTotal = expectedParcels * cartonsPerParcel * unitsPerCarton;
        if (calculatedTotal != TotalQuantity)
            throw new InvalidOperationException(
                $"Hierarquia inconsistente: {expectedParcels} parcels × {cartonsPerParcel} caixas × {unitsPerCarton} unidades = {calculatedTotal}, esperado {TotalQuantity}");
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementPackedParcels()
    {
        PackedParcels++;
        UpdatedAt = DateTime.UtcNow;
    }
}
