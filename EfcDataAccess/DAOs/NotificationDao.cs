using Application.DaoInterfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class NotificationDao : INotificationDao
{
    private readonly EfcContext _context;

    public NotificationDao(EfcContext context){
        _context = context;
    }

    public async Task<List<Notification>> GetNotificationsAsync(){
        return await _context.Notifications
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationByIdAsync(int id){
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Notification?> GetNotificationByMeasurementThresholdHigherAsync(int measurementId){
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Threshold >= measurementId);
    }

    public async Task<Notification?> GetNotificationByMeasurementThresholdLowerAsync(int measurementId){
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Threshold <= measurementId);
    }

    public async Task<Notification> AddNotificationAsync(Notification notification){
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
}