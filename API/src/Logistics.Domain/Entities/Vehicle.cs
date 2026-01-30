using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string LicensePlate { get; private set; }
    public string Model { get; private set; }
    public string? Brand { get; private set; }
    public string? VehicleType { get; private set; }
    public int Year { get; private set; }
    public decimal? Capacity { get; private set; }
    public VehicleStatus Status { get; private set; }
    
    // Tracking fields
    public string? TrackingToken { get; private set; }
    public bool TrackingEnabled { get; private set; }
    public double? LastLatitude { get; private set; }
    public double? LastLongitude { get; private set; }
    public DateTime? LastLocationUpdate { get; private set; }
    public double? CurrentSpeed { get; private set; }
    public string? CurrentAddress { get; private set; }
    public bool IsMoving { get; private set; }
    
    // Driver relationship
    public Guid? DriverId { get; private set; }
    public string? DriverName { get; private set; }
    public string? DriverPhone { get; private set; }
    
    // Current shipment being delivered
    public Guid? CurrentShipmentId { get; private set; }
    
    // Additional info
    public string? Color { get; private set; }
    public string? FuelType { get; private set; }
    public string? Notes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Company Company { get; private set; }
    public Driver? Driver { get; private set; }
    public OutboundShipment? CurrentShipment { get; private set; }

    // Constructor privado para EF
    private Vehicle() { }

    public Vehicle(Guid companyId, string licensePlate, string model, int year, string? brand = null, 
                   string? vehicleType = null, decimal? capacity = null, string? color = null, string? fuelType = null)
    {
        Id = Guid.NewGuid();
        CompanyId = companyId;
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        Brand = brand;
        VehicleType = vehicleType;
        Capacity = capacity;
        Color = color;
        FuelType = fuelType;
        Status = VehicleStatus.Available;
        TrackingEnabled = false;
        TrackingToken = GenerateTrackingToken();
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string licensePlate, string model, int year, string? brand = null, 
                       string? vehicleType = null, decimal? capacity = null, string? color = null, 
                       string? fuelType = null, string? notes = null)
    {
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        Brand = brand;
        VehicleType = vehicleType;
        Capacity = capacity;
        Color = color;
        FuelType = fuelType;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    public void UpdateStatus(VehicleStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableTracking()
    {
        TrackingEnabled = true;
        if (string.IsNullOrEmpty(TrackingToken))
            TrackingToken = GenerateTrackingToken();
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableTracking()
    {
        TrackingEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegenerateTrackingToken()
    {
        TrackingToken = GenerateTrackingToken();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(double latitude, double longitude, double? speed = null, string? address = null)
    {
        LastLatitude = latitude;
        LastLongitude = longitude;
        CurrentSpeed = speed;
        CurrentAddress = address;
        LastLocationUpdate = DateTime.UtcNow;
        IsMoving = speed.HasValue && speed.Value > 0;
        
        if (Status == VehicleStatus.Available)
            Status = VehicleStatus.InTransit;
    }

    public void SetMoving(bool isMoving)
    {
        IsMoving = isMoving;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDriver(Guid driverId, string driverName, string? driverPhone = null)
    {
        DriverId = driverId;
        DriverName = driverName;
        DriverPhone = driverPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDriver(string driverName, string? driverPhone = null)
    {
        DriverName = driverName;
        DriverPhone = driverPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDriver()
    {
        DriverId = null;
        DriverName = null;
        DriverPhone = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignShipment(Guid shipmentId)
    {
        CurrentShipmentId = shipmentId;
        Status = VehicleStatus.InTransit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteShipment()
    {
        CurrentShipmentId = null;
        Status = VehicleStatus.Available;
        IsMoving = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateTrackingToken()
    {
        return $"TRK-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(LicensePlate))
            throw new ArgumentException("Placa é obrigatória", nameof(LicensePlate));

        if (string.IsNullOrWhiteSpace(Model))
            throw new ArgumentException("Modelo é obrigatório", nameof(Model));

        if (Year < 1900 || Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Ano inválido", nameof(Year));

        if (CompanyId == Guid.Empty)
            throw new ArgumentException("CompanyId é obrigatório", nameof(CompanyId));
    }
}
