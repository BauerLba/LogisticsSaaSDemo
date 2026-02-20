using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Core.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _repository;

    public ShipmentService(IShipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Shipment>> GetActiveShipmentsAsync()
    {
        var shipments = await _repository.GetAllAsync();
        return shipments.Where(s => s.Status != Domain.Enums.ShipmentStatus.Delivered && s.Status != Domain.Enums.ShipmentStatus.Cancelled);
    }

    public async Task<Shipment?> TrackShipmentAsync(string trackingNumber)
    {
        return await _repository.GetByTrackingNumberAsync(trackingNumber);
    }

    public async Task CreateShipmentAsync(Shipment shipment)
    {
        shipment.TrackingNumber = $"TRK-{DateTime.UtcNow.Ticks.ToString().Substring(10)}-{new Random().Next(100, 999)}";
        await _repository.AddAsync(shipment);
    }
}
