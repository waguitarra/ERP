using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Serilog;

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
        Log.Information("[UnitOfWork] CONSTRUTOR - DbContext HashCode: {HashCode}", _context.GetHashCode());
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
        Log.Information("[UnitOfWork] ========== CommitAsync INICIO ==========");
        Log.Information("[UnitOfWork] DbContext HashCode: {HashCode}", _context.GetHashCode());
        
        var entriesCount = _context.ChangeTracker.Entries().Count();
        Log.Information("[UnitOfWork] ChangeTracker total entries: {Entries}", entriesCount);
        
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Log.Information("[UnitOfWork] Entry: {EntityType} - State: {State} - HashCode: {HashCode}", 
                entry.Entity.GetType().Name, entry.State, entry.Entity.GetHashCode());
        }
        
        if (entriesCount == 0)
        {
            Log.Warning("[UnitOfWork] ATENÇÃO: ChangeTracker está VAZIO - nada para salvar!");
        }
        
        Log.Information("[UnitOfWork] Chamando SaveChangesAsync()...");
        var result = await _context.SaveChangesAsync();
        Log.Information("[UnitOfWork] SaveChangesAsync() retornou: {Result} registros salvos", result);
        
        if (result == 0 && entriesCount > 0)
        {
            Log.Error("[UnitOfWork] ERRO CRÍTICO: Havia {Entries} entries mas SaveChanges retornou 0!", entriesCount);
        }
        
        Log.Information("[UnitOfWork] ========== CommitAsync FIM ==========");
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
