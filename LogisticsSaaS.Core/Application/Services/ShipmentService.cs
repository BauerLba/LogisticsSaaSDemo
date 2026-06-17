using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;
using LogisticsSaaS.Core.Domain.Enums;

namespace LogisticsSaaS.Core.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _repository;
    private readonly AuditService _auditService;

    public ShipmentService(IShipmentRepository repository, AuditService? auditService = null)
    {
        _repository = repository;
        _auditService = auditService ?? new AuditService();
    }

    public async Task<IEnumerable<Shipment>> GetAllShipmentsAsync()
    {
        return await _repository.GetAllAsync();
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
        _auditService.Log("CREATE", "Shipment", null, $"Created shipment {shipment.TrackingNumber} from {shipment.Origin} to {shipment.Destination}");
    }

    public async Task<(IEnumerable<Shipment> Items, int TotalCount)> GetShipmentsAsync(int page = 1, int pageSize = 10)
    {
        var allShipments = await GetActiveShipmentsAsync();
        var totalCount = allShipments.Count();
        var items = allShipments
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return (items, totalCount);
    }

    public async Task<int> GetShipmentCountAsync()
    {
        var shipments = await GetActiveShipmentsAsync();
        return shipments.Count();
    }

    public async Task<Dictionary<string, int>> GetStatusDistributionAsync()
    {
        var shipments = await GetActiveShipmentsAsync();
        return shipments
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());
    }

    public async Task<List<(string Date, int Count)>> GetShipmentsTimelineAsync(int days = 30)
    {
        var allShipments = await GetAllShipmentsAsync();
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return allShipments
            .Where(s => s.CreatedAt >= cutoffDate)
            .GroupBy(s => s.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => (g.Key.ToString("MMM dd"), g.Count()))
            .ToList();
    }

    public async Task<string> ExportToCSVAsync()
    {
        var shipments = await GetActiveShipmentsAsync();
        var csv = new System.Text.StringBuilder();

        csv.AppendLine("Tracking Number,Sender,Receiver,Origin,Destination,Weight,Status,Created At,Estimated Delivery");

        foreach (var shipment in shipments.OrderByDescending(s => s.CreatedAt))
        {
            csv.AppendLine($"\"{shipment.TrackingNumber}\",\"{shipment.SenderName}\",\"{shipment.ReceiverName}\",\"{shipment.Origin}\",\"{shipment.Destination}\",{shipment.Weight},\"{shipment.Status}\",\"{shipment.CreatedAt:yyyy-MM-dd HH:mm}\",\"{shipment.EstimatedDelivery?.ToShortDateString() ?? "TBD"}\"");
        }

        return csv.ToString();
    }

    public async Task UpdateStatusAsync(Guid id, ShipmentStatus newStatus)
    {
        await _repository.UpdateStatusAsync(id, newStatus);
        _auditService.Log("UPDATE", "Shipment", null, $"Updated shipment status to {newStatus}");
    }
}
