using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;

namespace Application.Logic;

public class NotificiationLogic : INotificationLogic
{
    private readonly INotificationDao _notificationDao;

    public NotificiationLogic(INotificationDao notificationDao)
    {
        _notificationDao = notificationDao;
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(int id){
        Notification? notification = await _notificationDao.GetNotificationByIdAsync(id);
        NotificationDto? notificationDto = new NotificationDto(notification.Threshold, notification.MeasurementID, notification.Message);
        

        return notificationDto;
    }

    public async Task<NotificationDto?> GetNotificationByMeasurementThresholdHigherAsync(int measurementId){
        Notification? notification = await _notificationDao.GetNotificationByMeasurementThresholdHigherAsync(measurementId);
        NotificationDto? notificationDto = new NotificationDto(notification.Threshold, notification.MeasurementID, notification.Message);

        return notificationDto;
    }

    public async Task<NotificationDto?> GetNotificationByMeasurementThresholdLowerAsync(int measurementId){
        Notification? notification = await _notificationDao.GetNotificationByMeasurementThresholdLowerAsync(measurementId);
        NotificationDto? notificationDto = new NotificationDto(notification.Threshold, notification.MeasurementID, notification.Message);

        return notificationDto;
    }

    public async Task<List<NotificationDto?>> GetNotificationsAsync(){
        List<Notification> notifications = await _notificationDao.GetNotificationsAsync();
        List<NotificationDto?> notificationDtos = new List<NotificationDto?>();
        foreach(Notification notification in notifications){
            NotificationDto notificationDto = new NotificationDto(notification.Threshold, notification.MeasurementID, notification.Message);
            notificationDtos.Add(notificationDto);
        }

        return notificationDtos;
    }

    public async Task<Notification> AddNotificationAsync(NotificationDto notificationDto){
        Notification notification = new Notification(notificationDto.Threshold, notificationDto.MeasurementID, notificationDto.Message);
        Notification addedNotification = await _notificationDao.AddNotificationAsync(notification);

        return addedNotification;
    }

}