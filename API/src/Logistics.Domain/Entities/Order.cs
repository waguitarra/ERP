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
    
    // Campos WMS
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Guid? OriginWarehouseId { get; private set; }
    public Guid? DestinationWarehouseId { get; private set; }
    
    // Geolocalização
    public string? ShippingZipCode { get; private set; }
    public decimal? ShippingLatitude { get; private set; }
    public decimal? ShippingLongitude { get; private set; }
    public string? ShippingCity { get; private set; }
    public string? ShippingState { get; private set; }
    public string? ShippingCountry { get; private set; }
    
    // Rastreamento
    public string? TrackingNumber { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }
    public DateTime? ActualDeliveryDate { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    
    // Purchase Order - Preços e Custos
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TaxPercentage { get; private set; }
    public decimal DesiredMarginPercentage { get; private set; }
    public decimal SuggestedSalePrice { get; private set; }
    public decimal EstimatedProfit { get; private set; }
    
    // Purchase Order - Hierarquia de Embalagem
    public int ExpectedParcels { get; private set; }
    public int ReceivedParcels { get; private set; }
    public int ExpectedCartons { get; private set; }
    public int UnitsPerCarton { get; private set; }
    public int CartonsPerParcel { get; private set; }
    
    // Purchase Order - Logística de Entrega
    public string? ShippingDistance { get; private set; }
    public decimal ShippingCost { get; private set; }
    public string? DockDoorNumber { get; private set; }
    
    // Purchase Order - Nacional vs Internacional
    public bool IsInternational { get; private set; }
    public string? OriginCountry { get; private set; }
    
    // Importação Internacional
    public string? PortOfEntry { get; private set; }
    public string? CustomsBroker { get; private set; }
    public bool IsOwnCarrier { get; private set; }
    public string? ThirdPartyCarrier { get; private set; }
    public string? ContainerNumber { get; private set; }
    public string? BillOfLading { get; private set; }
    public string? ImportLicenseNumber { get; private set; }
    public DateTime? EstimatedArrivalPort { get; private set; }
    public DateTime? ActualArrivalPort { get; private set; }
    public string? Incoterm { get; private set; }
    
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Customer? Customer { get; private set; }
    public Supplier? Supplier { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public Driver? Driver { get; private set; }
    public Warehouse? OriginWarehouse { get; private set; }
    public Warehouse? DestinationWarehouse { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public ICollection<OrderDocument> Documents { get; private set; } = new List<OrderDocument>();

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

    // Métodos WMS
    public void AssignVehicle(Guid vehicleId)
    {
        VehicleId = vehicleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDriver(Guid driverId)
    {
        DriverId = driverId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetWarehouses(Guid? originWarehouseId, Guid? destinationWarehouseId)
    {
        OriginWarehouseId = originWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLogistics(Guid? vehicleId, Guid? driverId, Guid? originWarehouseId, Guid? destinationWarehouseId)
    {
        VehicleId = vehicleId;
        DriverId = driverId;
        OriginWarehouseId = originWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetGeolocation(string? zipCode, decimal? latitude, decimal? longitude, string? city, string? state, string? country)
    {
        ShippingZipCode = zipCode;
        ShippingLatitude = latitude;
        ShippingLongitude = longitude;
        ShippingCity = city;
        ShippingState = state;
        ShippingCountry = country;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTracking(string? trackingNumber, DateTime? estimatedDeliveryDate, DateTime? actualDeliveryDate)
    {
        TrackingNumber = trackingNumber;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        ActualDeliveryDate = actualDeliveryDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingLocation(string? zipCode, decimal? latitude, decimal? longitude, string? city, string? state, string? country)
    {
        ShippingZipCode = zipCode;
        ShippingLatitude = latitude;
        ShippingLongitude = longitude;
        ShippingCity = city;
        ShippingState = state;
        ShippingCountry = country;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number não pode ser vazio");
        TrackingNumber = trackingNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEstimatedDeliveryDate(DateTime estimatedDate)
    {
        EstimatedDeliveryDate = estimatedDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsShipped()
    {
        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        ActualDeliveryDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Purchase Order Methods
    public void SetPurchaseDetails(decimal unitCost, decimal taxPercentage, decimal desiredMarginPercentage)
    {
        if (unitCost <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero");
        if (taxPercentage < 0 || taxPercentage > 100)
            throw new ArgumentException("Percentual de imposto inválido");
        if (desiredMarginPercentage < 0)
            throw new ArgumentException("Margem não pode ser negativa");

        UnitCost = unitCost;
        TaxPercentage = taxPercentage;
        DesiredMarginPercentage = desiredMarginPercentage;
        
        TotalCost = UnitCost * TotalQuantity;
        TaxAmount = TotalCost * (TaxPercentage / 100);
        
        var costWithTax = UnitCost + (UnitCost * (TaxPercentage / 100));
        SuggestedSalePrice = costWithTax + (costWithTax * (DesiredMarginPercentage / 100));
        EstimatedProfit = (SuggestedSalePrice - costWithTax) * TotalQuantity;
        
        UpdatedAt = DateTime.UtcNow;
    }
    
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
    
    public void SetShippingLogistics(string? distance, decimal shippingCost, string? dockDoorNumber)
    {
        ShippingDistance = distance;
        ShippingCost = shippingCost;
        DockDoorNumber = dockDoorNumber;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void IncrementReceivedParcels()
    {
        ReceivedParcels++;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetAsInternational(string originCountry, string portOfEntry, string containerNumber, string incoterm)
    {
        if (string.IsNullOrWhiteSpace(originCountry))
            throw new ArgumentException("País de origem é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(portOfEntry))
            throw new ArgumentException("Porto de entrada é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(containerNumber))
            throw new ArgumentException("Número do container é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(incoterm))
            throw new ArgumentException("Incoterm é obrigatório para importação");

        IsInternational = true;
        OriginCountry = originCountry;
        PortOfEntry = portOfEntry;
        ContainerNumber = containerNumber;
        Incoterm = incoterm;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetImportDetails(
        string? customsBroker,
        bool isOwnCarrier,
        string? thirdPartyCarrier,
        string? billOfLading,
        string? importLicenseNumber,
        DateTime? estimatedArrivalPort)
    {
        if (!IsInternational)
            throw new InvalidOperationException("Order não é internacional");

        CustomsBroker = customsBroker;
        IsOwnCarrier = isOwnCarrier;
        ThirdPartyCarrier = thirdPartyCarrier;
        BillOfLading = billOfLading;
        ImportLicenseNumber = importLicenseNumber;
        EstimatedArrivalPort = estimatedArrivalPort;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetActualPortArrival(DateTime arrivalDate)
    {
        if (!IsInternational)
            throw new InvalidOperationException("Order não é internacional");
        
        ActualArrivalPort = arrivalDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
