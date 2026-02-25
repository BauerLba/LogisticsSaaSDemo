using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;
using LogisticsSaaS.Core.Domain.Enums;
using LogisticsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsSaaS.Infrastructure.Repositories;

public class EfShipmentRepository : IShipmentRepository
{
    private readonly LogisticsDbContext _context;

    public EfShipmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return await _context.Shipments.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
    }

    public async Task AddAsync(Shipment shipment)
    {
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid id, ShipmentStatus status)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment != null)
        {
            shipment.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
