using Logistics.Application.DTOs.Receipt;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _repository;
    private readonly IInboundShipmentRepository _shipmentRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptService(
        IReceiptRepository repository,
        IInboundShipmentRepository shipmentRepository,
        IWarehouseRepository warehouseRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _shipmentRepository = shipmentRepository;
        _warehouseRepository = warehouseRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.InboundShipmentId);
        if (shipment == null) throw new KeyNotFoundException("Inbound Shipment não encontrado");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");

        var receipt = new Receipt(
            request.ReceiptNumber,
            request.InboundShipmentId,
            request.WarehouseId,
            request.ReceivedBy
        );

        // Buscar Order relacionado ao InboundShipment para criar ReceiptLines automaticamente
        var order = await _orderRepository.GetByIdAsync(shipment.OrderId);
        if (order != null && order.Items.Any())
        {  
            // Criar ReceiptLines baseado nos OrderItems
            foreach (var orderItem in order.Items)
            {
                var receiptLine = new ReceiptLine(
                    receipt.Id,
                    orderItem.ProductId,
                    orderItem.SKU,
                    orderItem.QuantityOrdered
                );
                receipt.AddLine(receiptLine);
            }
        }

        await _repository.AddAsync(receipt);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(receipt.Id);
    }

    public async Task<ReceiptResponse> GetByIdAsync(Guid id)
    {
        var receipt = await _repository.GetByIdAsync(id);
        if (receipt == null) throw new KeyNotFoundException("Receipt não encontrado");
        return MapToResponse(receipt);
    }

    public async Task<IEnumerable<ReceiptResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var receipts = await _repository.GetByWarehouseIdAsync(warehouseId);
        return receipts.Select(MapToResponse);
    }

    public async Task<IEnumerable<ReceiptResponse>> GetAllAsync()
    {
        var receipts = await _repository.GetAllAsync();
        return receipts.Select(MapToResponse);
    }

    private static ReceiptResponse MapToResponse(Receipt receipt)
    {
        return new ReceiptResponse(
            receipt.Id,
            receipt.ReceiptNumber,
            receipt.InboundShipmentId,
            receipt.InboundShipment?.ShipmentNumber ?? "",
            receipt.ReceiptDate,
            receipt.Status,
            receipt.WarehouseId,
            receipt.Warehouse?.Name ?? "",
            receipt.ReceivedBy,
            receipt.Lines.Select(l => new ReceiptLineResponse(
                l.Id,
                l.ProductId,
                l.SKU,
                l.Product?.Name ?? "",
                l.LotNumber,
                l.QuantityExpected,
                l.QuantityReceived,
                l.QuantityDamaged,
                l.InspectionStatus,
                l.ExpiryDate
            )).ToList(),
            receipt.CreatedAt
        );
    }
}
