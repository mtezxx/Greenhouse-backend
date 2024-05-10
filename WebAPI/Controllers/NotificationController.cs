using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase {
    private readonly INotificationLogic _notificationLogic;

    public NotificationController(INotificationLogic notificationLogic) {
        _notificationLogic = notificationLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications() {
        var notifications = await _notificationLogic.GetNotificationsAsync();
        if (notifications.Count > 0) {
            return Ok(notifications);
        }
        return NotFound("No notifications available.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id) {
        var notification = await _notificationLogic.GetNotificationByIdAsync(id);
        if (notification != null) {
            return Ok(notification);
        }
        return NotFound($"Notification with id: {id} not found.");
    }

    [HttpPost]
    public async Task<IActionResult> AddNotification([FromBody] NotificationDto notificationDto) {
        var (newNotificationDto, newNotificationId) = await _notificationLogic.AddNotificationAsync(notificationDto);
        return CreatedAtAction(nameof(GetNotificationById), new { id = newNotificationId }, newNotificationDto);
    }


    
    [HttpGet("bytype/{type}")]
    public async Task<IActionResult> GetNotificationByMeasurementType(string type) {
        var notification = await _notificationLogic.GetNotificationByMeasurementTypeAsync(type);
        if (notification != null) {
            return Ok(notification);
        }
        return NotFound($"No relevant notification found for measurement type {type}.");
    }
}