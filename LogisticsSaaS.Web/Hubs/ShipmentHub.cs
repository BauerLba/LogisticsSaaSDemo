using Microsoft.AspNetCore.SignalR;
using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Web.Hubs;

public class ShipmentHub : Hub
{
    public async Task JoinShipmentGroup(string trackingNumber)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"shipment-{trackingNumber}");
    }

    public async Task LeaveShipmentGroup(string trackingNumber)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"shipment-{trackingNumber}");
    }

    public async Task NotifyShipmentStatusUpdate(string trackingNumber, string newStatus)
    {
        await Clients.Group($"shipment-{trackingNumber}")
            .SendAsync("ShipmentStatusUpdated", new
            {
                TrackingNumber = trackingNumber,
                NewStatus = newStatus,
                UpdatedAt = DateTime.UtcNow
            });
    }

    public async Task NotifyNewShipment(Shipment shipment)
    {
        await Clients.All.SendAsync("NewShipmentCreated", new
        {
            shipment.TrackingNumber,
            shipment.SenderName,
            shipment.ReceiverName,
            shipment.Origin,
            shipment.Destination,
            shipment.CreatedAt
        });
    }

    public async Task NotifyShipmentDeleted(string trackingNumber)
    {
        await Clients.All.SendAsync("ShipmentDeleted", new { TrackingNumber = trackingNumber });
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveMessage", "Connected to shipment tracking service");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}