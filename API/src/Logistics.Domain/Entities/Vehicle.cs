using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string LicensePlate { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public VehicleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation property
    public Company Company { get; private set; }

    // Constructor privado para EF
    private Vehicle() { }

    public Vehicle(Guid companyId, string licensePlate, string model, int year)
    {
        Id = Guid.NewGuid();
        CompanyId = companyId;
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        Status = VehicleStatus.Available;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string licensePlate, string model, int year)
    {
        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    public void UpdateStatus(VehicleStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
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
