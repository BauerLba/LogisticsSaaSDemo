using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Core.Application.Interfaces;

public interface IShipmentRepository
{
    Task<IEnumerable<Shipment>> GetAllAsync();
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
    Task AddAsync(Shipment shipment);
    Task UpdateStatusAsync(Guid id, Domain.Enums.ShipmentStatus status);
}
