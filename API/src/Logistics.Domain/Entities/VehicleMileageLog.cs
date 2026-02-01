namespace Logistics.Domain.Entities;

/// <summary>
/// Registro de quilometragem do veículo com localização GPS
/// Usado para rastrear viagens, percursos e histórico de movimentação
/// </summary>
public class VehicleMileageLog
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CompanyId { get; private set; }
    
    // Tipo de registro
    public MileageLogType Type { get; private set; }
    
    // Quilometragem
    public decimal StartMileage { get; private set; }
    public decimal EndMileage { get; private set; }
    public decimal Distance => EndMileage - StartMileage;
    
    // Localização de saída
    public double? StartLatitude { get; private set; }
    public double? StartLongitude { get; private set; }
    public string? StartAddress { get; private set; }
    
    // Localização de chegada
    public double? EndLatitude { get; private set; }
    public double? EndLongitude { get; private set; }
    public string? EndAddress { get; private set; }
    
    // Datas
    public DateTime StartDateTime { get; private set; }
    public DateTime? EndDateTime { get; private set; }
    public TimeSpan? Duration => EndDateTime.HasValue ? EndDateTime.Value - StartDateTime : null;
    
    // Motorista
    public Guid? DriverId { get; private set; }
    public string? DriverName { get; private set; }
    
    // Entrega relacionada
    public Guid? ShipmentId { get; private set; }
    public string? ShipmentNumber { get; private set; }
    
    // Combustível
    public decimal? FuelConsumed { get; private set; }  // Litros
    public decimal? FuelCost { get; private set; }      // Custo
    public decimal? FuelEfficiency => FuelConsumed > 0 ? Distance / FuelConsumed : null; // Km/L
    
    // Status
    public MileageLogStatus Status { get; private set; }
    
    // Pedágio e custos extras
    public decimal? TollCost { get; private set; }
    public decimal? ParkingCost { get; private set; }
    public decimal? OtherCosts { get; private set; }
    public decimal TotalCost => (FuelCost ?? 0) + (TollCost ?? 0) + (ParkingCost ?? 0) + (OtherCosts ?? 0);
    
    // Propósito da viagem
    public string? Purpose { get; private set; }
    public string? Notes { get; private set; }
    
    // Polyline do percurso (encoded Google Polyline)
    public string? RoutePolyline { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public Vehicle Vehicle { get; private set; }
    public Company Company { get; private set; }
    public Driver? Driver { get; private set; }
    public OutboundShipment? Shipment { get; private set; }

    private VehicleMileageLog() { }

    public VehicleMileageLog(
        Guid vehicleId,
        Guid companyId,
        MileageLogType type,
        decimal startMileage,
        DateTime startDateTime,
        double? startLatitude = null,
        double? startLongitude = null,
        string? startAddress = null,
        Guid? driverId = null,
        string? driverName = null,
        Guid? shipmentId = null,
        string? shipmentNumber = null,
        string? purpose = null)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        CompanyId = companyId;
        Type = type;
        StartMileage = startMileage;
        StartDateTime = startDateTime;
        StartLatitude = startLatitude;
        StartLongitude = startLongitude;
        StartAddress = startAddress;
        DriverId = driverId;
        DriverName = driverName;
        ShipmentId = shipmentId;
        ShipmentNumber = shipmentNumber;
        Purpose = purpose;
        Status = MileageLogStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
    }

    public void CompleteTrip(
        decimal endMileage,
        DateTime endDateTime,
        double? endLatitude = null,
        double? endLongitude = null,
        string? endAddress = null,
        string? routePolyline = null)
    {
        EndMileage = endMileage;
        EndDateTime = endDateTime;
        EndLatitude = endLatitude;
        EndLongitude = endLongitude;
        EndAddress = endAddress;
        RoutePolyline = routePolyline;
        Status = MileageLogStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFuelInfo(
        decimal fuelConsumed,
        decimal fuelCost)
    {
        FuelConsumed = fuelConsumed;
        FuelCost = fuelCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCosts(
        decimal? tollCost = null,
        decimal? parkingCost = null,
        decimal? otherCosts = null)
    {
        TollCost = tollCost;
        ParkingCost = parkingCost;
        OtherCosts = otherCosts;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        Status = MileageLogStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        MileageLogType type,
        string? purpose,
        string? notes)
    {
        Type = type;
        Purpose = purpose;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum MileageLogType
{
    Delivery = 0,       // Entrega
    Pickup = 1,         // Coleta
    Transfer = 2,       // Transferência
    Maintenance = 3,    // Ida para manutenção
    Inspection = 4,     // Inspeção
    Refueling = 5,      // Abastecimento
    Administrative = 6, // Administrativo
    Personal = 7,       // Uso pessoal (se permitido)
    Other = 99
}

public enum MileageLogStatus
{
    InProgress = 0,    // Em andamento
    Completed = 1,     // Concluído
    Cancelled = 2      // Cancelado
}
