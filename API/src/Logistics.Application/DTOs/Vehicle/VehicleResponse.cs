namespace Logistics.Application.DTOs.Vehicle;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? VehicleType { get; set; }
    public int Year { get; set; }
    public decimal? Capacity { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    
    // Tracking info
    public string? TrackingToken { get; set; }
    public bool TrackingEnabled { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    public double? CurrentSpeed { get; set; }
    public string? CurrentAddress { get; set; }
    public bool IsMoving { get; set; }
    
    // Driver info
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? DriverLicenseNumber { get; set; }
    
    // Current shipment info
    public Guid? CurrentShipmentId { get; set; }
    public string? CurrentShipmentNumber { get; set; }
    public string? CurrentShipmentAddress { get; set; }
    public string? CurrentShipmentStatus { get; set; }
    public string? CurrentOrderCustomerName { get; set; }
    public string? CurrentOrderCustomerPhone { get; set; }
    
    // Additional info
    public string? Color { get; set; }
    public string? FuelType { get; set; }
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Company info
    public string? CompanyName { get; set; }
}
