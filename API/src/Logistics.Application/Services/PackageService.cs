using Logistics.Application.DTOs.Package;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _repository;
    private readonly IPackingTaskRepository _packingTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PackageService(
        IPackageRepository repository,
        IPackingTaskRepository packingTaskRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _packingTaskRepository = packingTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PackageResponse> CreateAsync(CreatePackageRequest request)
    {
        if (await _packingTaskRepository.GetByIdAsync(request.PackingTaskId) == null)
            throw new KeyNotFoundException("Tarefa de embalagem não encontrada");

        if (await _repository.GetByTrackingNumberAsync(request.TrackingNumber) != null)
            throw new InvalidOperationException("Tracking number já existe");

        var package = new Package(
            request.PackingTaskId,
            request.TrackingNumber,
            request.Type
        );

        package.SetDimensions(request.Weight, request.Length, request.Width, request.Height);

        await _repository.AddAsync(package);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(package.Id);
    }

    public async Task<PackageResponse> GetByIdAsync(Guid id)
    {
        var package = await _repository.GetByIdAsync(id);
        if (package == null) throw new KeyNotFoundException("Pacote não encontrado");
        return MapToResponse(package);
    }

    public async Task<PackageResponse> GetByTrackingNumberAsync(string trackingNumber)
    {
        var package = await _repository.GetByTrackingNumberAsync(trackingNumber);
        if (package == null) throw new KeyNotFoundException("Pacote não encontrado");
        return MapToResponse(package);
    }

    public async Task<IEnumerable<PackageResponse>> GetByPackingTaskIdAsync(Guid packingTaskId)
    {
        var packages = await _repository.GetByPackingTaskIdAsync(packingTaskId);
        return packages.Select(MapToResponse);
    }

    public async Task ShipPackageAsync(Guid id)
    {
        var package = await _repository.GetByIdAsync(id);
        if (package == null) throw new KeyNotFoundException("Pacote não encontrado");

        package.SetStatus(PackageStatus.Shipped);
        await _repository.UpdateAsync(package);
        await _unitOfWork.CommitAsync();
    }

    private static PackageResponse MapToResponse(Package package)
    {
        return new PackageResponse(
            package.Id,
            package.PackingTaskId,
            package.TrackingNumber,
            package.Type,
            package.Weight,
            package.Length,
            package.Width,
            package.Height,
            package.Status,
            package.CreatedAt
        );
    }
}
