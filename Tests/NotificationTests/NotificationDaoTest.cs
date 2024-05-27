using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DaoInterfaces;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Tests.Utils;
using Xunit;

public class NotificationDaoTest : DbTestBase
{
    private INotificationDao _notificationDao;

    public NotificationDaoTest()
    {
        TestInit();
        TestInitialize();
    }

    public void TestInitialize()
    {
        _notificationDao = new NotificationDao(DbContext);
    }

    [Fact]
    public async Task GetNotificationsAsync_ReturnsListOfNotifications()
    {
        var notifications = new List<Notification>
        {
            new Notification { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" },
            new Notification { Threshold = 20, MeasurementType = "Humidity", Message = "Message2" }
        };
        DbContext.Notifications.AddRange(notifications);
        await DbContext.SaveChangesAsync();

        var result = await _notificationDao.GetNotificationsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Humidity", result[0].MeasurementType);
        Assert.Equal("Temperature", result[1].MeasurementType);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ReturnsNotification_WhenNotificationExists()
    {
        var notification = new Notification { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };
        DbContext.Notifications.Add(notification);
        await DbContext.SaveChangesAsync();

        var result = await _notificationDao.GetNotificationByIdAsync(notification.Id);

        Assert.NotNull(result);
        Assert.Equal(notification.Threshold, result.Threshold);
        Assert.Equal(notification.MeasurementType, result.MeasurementType);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ReturnsNull_WhenNotificationDoesNotExist()
    {
        var result = await _notificationDao.GetNotificationByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetLatestNotificationByTypeAsync_ReturnsLatestNotification()
    {
        var notification1 = new Notification { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };
        var notification2 = new Notification { Threshold = 20, MeasurementType = "Temperature", Message = "Message2" };
        DbContext.Notifications.AddRange(notification1, notification2);
        await DbContext.SaveChangesAsync();

        var result = await _notificationDao.GetLatestNotificationByTypeAsync("Temperature");

        Assert.NotNull(result);
        Assert.Equal(notification2.Threshold, result.Threshold);
        Assert.Equal(notification2.MeasurementType, result.MeasurementType);
    }

    [Fact]
    public async Task AddNotificationAsync_AddsNotificationSuccessfully()
    {
        var notification = new Notification { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };

        var result = await _notificationDao.AddNotificationAsync(notification);
        await DbContext.SaveChangesAsync();

        Assert.NotNull(result);
        Assert.Equal(notification.Threshold, result.Threshold);
        var notificationInDb = await DbContext.Notifications.FirstOrDefaultAsync();
        Assert.NotNull(notificationInDb);
        Assert.Equal(notification.Threshold, notificationInDb.Threshold);
    }

    [Fact]
    public async Task GetLatestMeasurementValueByTypeAsync_ReturnsLatestMeasurementValue()
    {
        var measurement1 = new Temperature { Value = 15, Time = DateTime.Now.AddMinutes(-10) };
        var measurement2 = new Temperature { Value = 20, Time = DateTime.Now };
        DbContext.Measurements.AddRange(measurement1, measurement2);
        await DbContext.SaveChangesAsync();

        var result = await _notificationDao.GetLatestMeasurementValueByTypeAsync("Temperature");

        Assert.Equal(20, result);
    }

    [Fact]
    public async Task GetLatestMeasurementValueByTypeAsync_ReturnsNull_WhenNoMeasurementsExist()
    {
        var result = await _notificationDao.GetLatestMeasurementValueByTypeAsync("Temperature");

        Assert.Null(result);
    }
}
