using Logistics.Application.DTOs.OutboundShipment;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class OutboundShipmentService : IOutboundShipmentService
{
    private readonly IOutboundShipmentRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutboundShipmentService(
        IOutboundShipmentRepository repository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OutboundShipmentResponse> CreateAsync(CreateOutboundShipmentRequest request)
    {
        if (await _orderRepository.GetByIdAsync(request.OrderId) == null)
            throw new KeyNotFoundException("Pedido não encontrado");

        if (await _repository.GetByShipmentNumberAsync(request.ShipmentNumber) != null)
            throw new InvalidOperationException("Número de expedição já existe");

        var shipment = new OutboundShipment(
            request.ShipmentNumber,
            request.OrderId,
            request.CarrierId
        );

        if (!string.IsNullOrEmpty(request.TrackingNumber))
            shipment.SetTracking(request.TrackingNumber);

        await _repository.AddAsync(shipment);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(shipment.Id);
    }

    public async Task<OutboundShipmentResponse> GetByIdAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");
        return MapToResponse(shipment);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetAllAsync()
    {
        var shipments = await _repository.GetAllWithDetailsAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetByOrderIdAsync(Guid orderId)
    {
        var shipments = await _repository.GetByOrderIdAsync(orderId);
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetByStatusAsync(OutboundStatus status)
    {
        var shipments = await _repository.GetByStatusAsync(status);
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetPendingAsync()
    {
        var shipments = await _repository.GetPendingAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetShippedAsync()
    {
        var shipments = await _repository.GetShippedAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task<IEnumerable<OutboundShipmentResponse>> GetInTransitAsync()
    {
        var shipments = await _repository.GetInTransitAsync();
        return shipments.Select(MapToResponse);
    }

    public async Task MarkReadyToShipAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        shipment.MarkReadyToShip();
        await _repository.UpdateAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    public async Task ShipAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        shipment.Ship(DateTime.UtcNow);
        await _repository.UpdateAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    public async Task MarkInTransitAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        shipment.MarkInTransit();
        await _repository.UpdateAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeliverAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        shipment.Deliver(DateTime.UtcNow);
        await _repository.UpdateAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        shipment.Cancel();
        await _repository.UpdateAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var shipment = await _repository.GetByIdAsync(id);
        if (shipment == null) throw new KeyNotFoundException("Expedição não encontrada");

        await _repository.DeleteAsync(shipment);
        await _unitOfWork.CommitAsync();
    }

    private static OutboundShipmentResponse MapToResponse(OutboundShipment shipment)
    {
        return new OutboundShipmentResponse(
            shipment.Id,
            shipment.ShipmentNumber,
            shipment.OrderId,
            shipment.Order?.OrderNumber ?? "",
            shipment.CarrierId,
            shipment.TrackingNumber,
            (int)shipment.Status,
            shipment.Status.ToString(),
            shipment.ShippedDate,
            shipment.DeliveredDate,
            shipment.DeliveryAddress,
            shipment.CreatedAt
        );
    }
}
