using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;

namespace Logistics.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LogisticsDbContext _context;
    private ICompanyRepository? _companies;
    private IUserRepository? _users;
    private IVehicleRepository? _vehicles;
    private IDriverRepository? _drivers;

    public UnitOfWork(LogisticsDbContext context)
    {
        _context = context;
    }

    public ICompanyRepository Companies
    {
        get { return _companies ??= new CompanyRepository(_context); }
    }

    public IUserRepository Users
    {
        get { return _users ??= new UserRepository(_context); }
    }

    public IVehicleRepository Vehicles
    {
        get { return _vehicles ??= new VehicleRepository(_context); }
    }

    public IDriverRepository Drivers
    {
        get { return _drivers ??= new DriverRepository(_context); }
    }

    public async Task<int> CommitAsync()
    {
        var entriesCount = _context.ChangeTracker.Entries().Count();
        Console.WriteLine($"[UnitOfWork] CommitAsync - Entries: {entriesCount}");
        
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Console.WriteLine($"[UnitOfWork] Entry: {entry.Entity.GetType().Name} - State: {entry.State}");
        }
        
        var result = await _context.SaveChangesAsync();
        Console.WriteLine($"[UnitOfWork] SaveChanges result: {result}");
        return result;
    }

    public async Task RollbackAsync()
    {
        await Task.Run(() =>
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        });
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
