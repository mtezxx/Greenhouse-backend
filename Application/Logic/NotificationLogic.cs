using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;

namespace Application.Logic;

public class NotificationLogic : INotificationLogic {
    private readonly INotificationDao _notificationDao;

    public NotificationLogic(INotificationDao notificationDao) {
        _notificationDao = notificationDao;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync() {
        var notifications = await _notificationDao.GetNotificationsAsync();
        return notifications.Select(n => new NotificationDto(n.Threshold, n.MeasurementType, n.Message)).ToList();
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(int id) {
        var notification = await _notificationDao.GetNotificationByIdAsync(id);
        if (notification != null) {
            return new NotificationDto(notification.Threshold, notification.MeasurementType, notification.Message);
        }
        return null;
    }

    public async Task<(NotificationDto, int)> AddNotificationAsync(NotificationDto notificationDto) {
        var notification = new Notification {
            Threshold = notificationDto.Threshold,
            MeasurementType = notificationDto.MeasurementType,
            Message = notificationDto.Message
        };

        var addedNotification = await _notificationDao.AddNotificationAsync(notification);

        var notificationDtoResult = new NotificationDto {
            Threshold = addedNotification.Threshold,
            MeasurementType = addedNotification.MeasurementType,
            Message = addedNotification.Message
        };
        return (notificationDtoResult, addedNotification.Id);
    }

    
    public async Task<NotificationDto?> GetNotificationByMeasurementTypeAsync(string type) {
        var latestMeasurementValue = await _notificationDao.GetLatestMeasurementValueByTypeAsync(type);
        if (!latestMeasurementValue.HasValue)
            return null; // No measurement data available

        var latestNotification = await _notificationDao.GetLatestNotificationByTypeAsync(type);
        if (latestNotification == null)
            return null; // No notification setting available

        // Check if the latest measurement value is higher or lower than the threshold
        string message;
        if (latestMeasurementValue > latestNotification.Threshold) {
            message = $"Latest {type} of {latestMeasurementValue.Value} is higher than the threshold of {latestNotification.Threshold}.";
        } else {
            message = $"Latest {type} of {latestMeasurementValue.Value} is lower than the threshold of {latestNotification.Threshold}.";
        }

        return new NotificationDto(latestNotification.Threshold, type, message);
    }
}