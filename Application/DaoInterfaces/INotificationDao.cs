using Domain.Entity;

namespace Application.DaoInterfaces;

public interface INotificationDao{
    Task<List<Notification>> GetNotificationsAsync();
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<Notification?> GetNotificationByMeasurementThresholdHigherAsync(int measurementId);
    Task<Notification?> GetNotificationByMeasurementThresholdLowerAsync(int measurementId);
    Task<Notification> AddNotificationAsync(Notification notification);
}