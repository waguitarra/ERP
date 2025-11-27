using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PurchaseOrder
{
    private PurchaseOrder() { } // EF Core

    public PurchaseOrder(Guid companyId, string purchaseOrderNumber, Guid supplierId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        if (string.IsNullOrWhiteSpace(purchaseOrderNumber))
            throw new ArgumentException("Número do pedido de compra não pode ser vazio");
        if (supplierId == Guid.Empty)
            throw new ArgumentException("SupplierId é obrigatório para Purchase Order");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        PurchaseOrderNumber = purchaseOrderNumber;
        SupplierId = supplierId;
        OrderDate = DateTime.UtcNow;
        Priority = OrderPriority.Normal;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string PurchaseOrderNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public OrderPriority Priority { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? SpecialInstructions { get; private set; }
    
    // Preços e Custos (COMPRA)
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TaxPercentage { get; private set; }
    public decimal DesiredMarginPercentage { get; private set; }
    public decimal SuggestedSalePrice { get; private set; }
    public decimal EstimatedProfit { get; private set; }
    
    // Hierarquia de Embalagem (RECEBIMENTO)
    public int ExpectedParcels { get; private set; }
    public int ReceivedParcels { get; private set; }
    public int ExpectedCartons { get; private set; }
    public int UnitsPerCarton { get; private set; }
    public int CartonsPerParcel { get; private set; }
    
    // Logística (WMS)
    public Guid? DestinationWarehouseId { get; private set; }  // Armazém que vai receber
    public Guid? VehicleId { get; private set; }               // Veículo que está trazendo
    public Guid? DriverId { get; private set; }                // Motorista
    public string? DockDoorNumber { get; private set; }        // Dock door de recebimento
    public string? ShippingDistance { get; private set; }
    public decimal ShippingCost { get; private set; }
    
    // Internacional
    public bool IsInternational { get; private set; }
    public string? OriginCountry { get; private set; }
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
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Supplier Supplier { get; private set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();
    public ICollection<PurchaseOrderDocument> Documents { get; private set; } = new List<PurchaseOrderDocument>();

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

    public void AddItem(PurchaseOrderItem item)
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

    public void SetAsInternational(string originCountry, string portOfEntry, string containerNumber, string incoterm)
    {
        if (string.IsNullOrWhiteSpace(originCountry))
            throw new ArgumentException("País de origem é obrigatório");
        if (string.IsNullOrWhiteSpace(portOfEntry))
            throw new ArgumentException("Porto de entrada é obrigatório");

        IsInternational = true;
        OriginCountry = originCountry;
        PortOfEntry = portOfEntry;
        ContainerNumber = containerNumber;
        Incoterm = incoterm;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementReceivedParcels()
    {
        ReceivedParcels++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLogistics(Guid? destinationWarehouseId, Guid? vehicleId, Guid? driverId, string? dockDoorNumber)
    {
        DestinationWarehouseId = destinationWarehouseId;
        VehicleId = vehicleId;
        DriverId = driverId;
        DockDoorNumber = dockDoorNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingDetails(string? distance, decimal shippingCost)
    {
        ShippingDistance = distance;
        ShippingCost = shippingCost;
        UpdatedAt = DateTime.UtcNow;
    }
}
