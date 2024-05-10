using Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.LogicInterfaces;

public interface INotificationLogic {
    Task<List<NotificationDto>> GetNotificationsAsync();
    Task<NotificationDto?> GetNotificationByIdAsync(int id);
    Task<(NotificationDto, int)> AddNotificationAsync(NotificationDto notificationDto);
    Task<NotificationDto?> GetNotificationByMeasurementTypeAsync(string type);
}