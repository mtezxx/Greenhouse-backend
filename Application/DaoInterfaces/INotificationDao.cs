using Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.DaoInterfaces;

public interface INotificationDao {
    Task<List<Notification>> GetNotificationsAsync();
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<Notification?> GetLatestNotificationByTypeAsync(string measurementType);
    Task<Notification> AddNotificationAsync(Notification notification);
    Task<double?> GetLatestMeasurementValueByTypeAsync(string measurementType);

}