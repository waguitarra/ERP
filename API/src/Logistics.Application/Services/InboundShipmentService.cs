using Logistics.Application.DTOs.InboundShipment;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class InboundShipmentService : IInboundShipmentService
{
    private readonly IInboundShipmentRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InboundShipmentService(
        IInboundShipmentRepository repository,
        IOrderRepository orderRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InboundShipmentResponse> CreateAsync(CreateInboundShipmentRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order == null) throw new KeyNotFoundException("Pedido não encontrado");

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);
        if (supplier == null) throw new KeyNotFoundException("Fornecedor não encontrado");

        var shipment = new InboundShipment(
            request.CompanyId,
            request.ShipmentNumber,
            request.OrderId,
            request.SupplierId
        );

        if (request.VehicleId.HasValue || request.DriverId.HasValue)
        {
            shipment.SetVehicleAndDriver(request.VehicleId, request.DriverId);
        }

        if (request.ExpectedArrivalDate.HasValue)
        {
            shipment.SetExpectedArrival(request.ExpectedArrivalDate.Value, request.DockDoorNumber);
        }

        if (!string.IsNullOrEmpty(request.ASNNumber))
        {
            shipment.SetASN(request.ASNNumber);
        }

        await _repository.AddAsync(shipment);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(shipment.Id);
    }

    public async Task<InboundShipmentResponse> GetByIdAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Inbound Shipment não encontrado");
        return MapToResponse(shipment);
    }

    public async Task<IEnumerable<InboundShipmentResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var shipments = await _repository.GetByCompanyIdAsync(companyId);
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<InboundShipmentResponse>> GetAllAsync()
    {
        var shipments = await _repository.GetAllWithDetailsAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<InboundShipmentResponse>> GetByStatusAsync(InboundStatus status)
    {
        var shipments = await _repository.GetByStatusAsync(status);
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<InboundShipmentResponse>> GetScheduledAsync()
    {
        var shipments = await _repository.GetScheduledAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<InboundShipmentResponse>> GetInProgressAsync()
    {
        var shipments = await _repository.GetInProgressAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task ReceiveAsync(Guid id, Guid receivedBy)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Inbound Shipment não encontrado");

        shipment.SetActualArrival(DateTime.UtcNow, receivedBy);
        await _unitOfWork.CommitAsync();
    }

    public async Task CompleteAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Inbound Shipment não encontrado");

        shipment.Complete();
        await _unitOfWork.CommitAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Inbound Shipment não encontrado");

        shipment.Cancel();
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    private static InboundShipmentResponse MapToResponse(InboundShipment shipment)
    {
        return new InboundShipmentResponse(
            shipment.Id,
            shipment.CompanyId,
            shipment.Company?.Name ?? "",
            shipment.ShipmentNumber,
            shipment.OrderId,
            shipment.Order?.OrderNumber ?? "",
            shipment.SupplierId,
            shipment.Supplier?.Name ?? "",
            shipment.VehicleId,
            shipment.Vehicle?.LicensePlate ?? null,
            shipment.DriverId,
            shipment.Driver?.Name ?? null,
            shipment.ExpectedArrivalDate,
            shipment.ActualArrivalDate,
            shipment.DockDoorNumber,
            (int)shipment.Status,
            shipment.Status.ToString(),
            shipment.TotalQuantityExpected,
            shipment.TotalQuantityReceived,
            shipment.ASNNumber,
            shipment.HasQualityIssues,
            shipment.CreatedAt,
            shipment.UpdatedAt
        );
    }
}
