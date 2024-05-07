using Domain.DTOs;
using Domain.Entity;

namespace Application.LogicInterfaces;

public interface INotificationLogic{
    Task<NotificationDto?> GetNotificationByIdAsync(int id);
    Task<NotificationDto?> GetNotificationByMeasurementThresholdHigherAsync(int measurementId);
    Task<NotificationDto?> GetNotificationByMeasurementThresholdLowerAsync(int measurementId);
    Task<List<NotificationDto?>> GetNotificationsAsync();
    Task<Notification> AddNotificationAsync(NotificationDto notificationDto);
}