using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;
using LogisticsSaaS.Core.Domain.Enums;

namespace LogisticsSaaS.Core.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _repository;

    public ShipmentService(IShipmentRepository repository)
    {
        _repository = repository;
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
}
