using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Data;

public class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<StorageLocation> StorageLocations { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    
    // WMS Entities
    public DbSet<WarehouseZone> WarehouseZones { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Lot> Lots { get; set; }
    
    // WMS Receiving (Inbound)
    public DbSet<InboundShipment> InboundShipments { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptLine> ReceiptLines { get; set; }
    public DbSet<PutawayTask> PutawayTasks { get; set; }
    
    // WMS Picking
    public DbSet<PickingWave> PickingWaves { get; set; }
    public DbSet<PickingTask> PickingTasks { get; set; }
    public DbSet<PickingLine> PickingLines { get; set; }
    
    // WMS Packing & Shipping
    public DbSet<PackingTask> PackingTasks { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<OutboundShipment> OutboundShipments { get; set; }
    
    // WMS Inventory
    public DbSet<SerialNumber> SerialNumbers { get; set; }
    public DbSet<CycleCount> CycleCounts { get; set; }
    
    // WMS Dock Management
    public DbSet<VehicleAppointment> VehicleAppointments { get; set; }
    public DbSet<DockDoor> DockDoors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticsDbContext).Assembly);
    }
}
