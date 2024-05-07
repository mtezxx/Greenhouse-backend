using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationLogic _notificationLogic;

    public NotificationController(INotificationLogic notificationLogic)
    {
        _notificationLogic = notificationLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _notificationLogic.GetNotificationsAsync();
        if (notifications.Count > 0)
        {
            return Ok(notifications);
        }
        return NotFound($"No notifications available.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var notification = await _notificationLogic.GetNotificationByIdAsync(id);
        if (notification != null)
        {
            return Ok(notification);
        }
        return NotFound($"Notification with id: {id} not found.");
    }

    [HttpGet("measurement/higher")]
    public async Task<IActionResult> GetNotificationByMeasurementThresholdHigher([FromBody] int measurementId)
    {
        var notification = await _notificationLogic.GetNotificationByMeasurementThresholdHigherAsync(measurementId);
        if (notification != null)
        {
            return Ok(notification);
        }
        return NotFound($"No notifications found for measurement with id {measurementId}.");
    }

    [HttpGet("measurement/lower")]
    public async Task<IActionResult> GetNotificationByMeasurementThresholdLower([FromBody] int measurementId)
    {
        var notification = await _notificationLogic.GetNotificationByMeasurementThresholdLowerAsync(measurementId);
        if (notification != null)
        {
            return Ok(notification);
        }
        return NotFound($"No notifications found for measurement with id {measurementId}.");
    }

    [HttpPost]
    public async Task<IActionResult> AddNotification([FromBody] NotificationDto notification)
    {
        var newNotification = await _notificationLogic.AddNotificationAsync(notification);
        return Created($"/notification/{newNotification.Id}", newNotification);
    }
}