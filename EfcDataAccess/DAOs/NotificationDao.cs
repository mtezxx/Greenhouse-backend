using Application.DaoInterfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class NotificationDao : INotificationDao
{
    private readonly EfcContext _context;

    public NotificationDao(EfcContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        return await _context.Notifications
            .OrderByDescending(n => n.Id)  // Order by Id in descending order
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _context.Notifications.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Notification?> GetLatestNotificationByTypeAsync(string measurementType)
    {
        return await _context.Notifications
            .Where(n => n.MeasurementType == measurementType)
            .OrderByDescending(n => n.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
    public async Task<double?> GetLatestMeasurementValueByTypeAsync(string measurementType) {
        var measurement = await _context.Measurements
            .Where(m => m.Type == measurementType)
            .OrderByDescending(m => m.Time)
            .FirstOrDefaultAsync();
        return measurement?.Value;
    }
    
}