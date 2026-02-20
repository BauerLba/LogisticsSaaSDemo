using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;
using LogisticsSaaS.Core.Domain.Enums;

namespace LogisticsSaaS.Infrastructure.Repositories;

public class InMemoryShipmentRepository : IShipmentRepository
{
    private readonly List<Shipment> _shipments = new()
    {
        new Shipment
        {
            TrackingNumber = "TRK-DEMO-123",
            SenderName = "Global Tech",
            ReceiverName = "Apple Store",
            Origin = "Berlin, DE",
            Destination = "Munich, DE",
            Weight = 15.5,
            Status = ShipmentStatus.InTransit,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            EstimatedDelivery = DateTime.UtcNow.AddDays(1)
        },
        new Shipment
        {
            TrackingNumber = "TRK-DEMO-456",
            SenderName = "Eco Goods",
            ReceiverName = "Lucas Müller",
            Origin = "Hamburg, DE",
            Destination = "Berlin, DE",
            Weight = 2.0,
            Status = ShipmentStatus.Processing,
            CreatedAt = DateTime.UtcNow.AddHours(-5),
            EstimatedDelivery = DateTime.UtcNow.AddDays(2)
        },
        new Shipment
        {
            TrackingNumber = "TRK-DEMO-789",
            SenderName = "Fast Logistics",
            ReceiverName = "Sophia Schmidt",
            Origin = "Paris, FR",
            Destination = "Berlin, DE",
            Weight = 45.0,
            Status = ShipmentStatus.Pending,
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        }
    };

    public Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Shipment>>(_shipments);
    }

    public Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = _shipments.FirstOrDefault(s => s.TrackingNumber.Equals(trackingNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(shipment);
    }

    public Task AddAsync(Shipment shipment)
    {
        _shipments.Add(shipment);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid id, ShipmentStatus status)
    {
        var shipment = _shipments.FirstOrDefault(s => s.Id == id);
        if (shipment != null)
        {
            shipment.Status = status;
        }
        return Task.CompletedTask;
    }
}
