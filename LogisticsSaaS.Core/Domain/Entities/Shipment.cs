using LogisticsSaaS.Core.Domain.Enums;

namespace LogisticsSaaS.Core.Domain.Entities;

public class Shipment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TrackingNumber { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public double Weight { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EstimatedDelivery { get; set; }
}
