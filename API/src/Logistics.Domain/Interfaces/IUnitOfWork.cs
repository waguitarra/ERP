namespace Logistics.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IUserRepository Users { get; }
    IVehicleRepository Vehicles { get; }
    IDriverRepository Drivers { get; }
    
    Task<int> CommitAsync();
    Task RollbackAsync();
}
