using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Moq;

namespace Tests.NotificationTests;

public class NotificationLogicTest
{
    private readonly Mock<INotificationDao> _notificationDaoMock;
    private readonly NotificationLogic _notificationLogic;

    public NotificationLogicTest()
    {
        _notificationDaoMock = new Mock<INotificationDao>();
        _notificationLogic = new NotificationLogic(
            _notificationDaoMock.Object
        );
    }
    
     [Fact]
    public async Task GetNotificationsAsync_ReturnsListOfNotificationDtos()
    {
        var notifications = new List<Notification>
        {
            new Notification { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" },
            new Notification { Threshold = 20, MeasurementType = "Humidity", Message = "Message2" }
        };

        _notificationDaoMock.Setup(x => x.GetNotificationsAsync())
            .ReturnsAsync(notifications);

        var result = await _notificationLogic.GetNotificationsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Temperature", result[0].MeasurementType);
        Assert.Equal("Humidity", result[1].MeasurementType);
        _notificationDaoMock.Verify(x => x.GetNotificationsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ReturnsNotificationDto_WhenNotificationExists()
    {
        var notification = new Notification { Id = 1, Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };

        _notificationDaoMock.Setup(x => x.GetNotificationByIdAsync(1))
            .ReturnsAsync(notification);

        var result = await _notificationLogic.GetNotificationByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(10, result.Threshold);
        Assert.Equal("Temperature", result.MeasurementType);
        _notificationDaoMock.Verify(x => x.GetNotificationByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ReturnsNull_WhenNotificationDoesNotExist()
    {
        _notificationDaoMock.Setup(x => x.GetNotificationByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Notification)null);

        var result = await _notificationLogic.GetNotificationByIdAsync(1);

        Assert.Null(result);
        _notificationDaoMock.Verify(x => x.GetNotificationByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task AddNotificationAsync_AddsNotificationSuccessfully()
    {
        var notificationDto = new NotificationDto { Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };
        var notification = new Notification { Threshold = notificationDto.Threshold, MeasurementType = notificationDto.MeasurementType, Message = notificationDto.Message };
        var addedNotification = new Notification { Id = 1, Threshold = 10, MeasurementType = "Temperature", Message = "Message1" };

        _notificationDaoMock.Setup(x => x.AddNotificationAsync(It.IsAny<Notification>()))
            .ReturnsAsync(addedNotification);

        var result = await _notificationLogic.AddNotificationAsync(notificationDto);

        Assert.NotNull(result);
        Assert.Equal(notificationDto.Threshold, result.Item1.Threshold);
        Assert.Equal(notificationDto.MeasurementType, result.Item1.MeasurementType);
        Assert.Equal(1, result.Item2);
        _notificationDaoMock.Verify(x => x.AddNotificationAsync(It.Is<Notification>(n => n.Threshold == notificationDto.Threshold && n.MeasurementType == notificationDto.MeasurementType)), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByMeasurementTypeAsync_ReturnsNotificationDto_WhenThresholdExceeded()
    {
        var measurementType = "Temperature";
        var latestValue = 15;
        var notification = new Notification { Threshold = 10, MeasurementType = measurementType, Message = "Message1" };

        _notificationDaoMock.Setup(x => x.GetLatestMeasurementValueByTypeAsync(measurementType))
            .ReturnsAsync(latestValue);

        _notificationDaoMock.Setup(x => x.GetLatestNotificationByTypeAsync(measurementType))
            .ReturnsAsync(notification);

        var result = await _notificationLogic.GetNotificationByMeasurementTypeAsync(measurementType);

        Assert.NotNull(result);
        Assert.Equal("Latest Temperature of 15 is higher than the threshold of 10.", result.Message);
        _notificationDaoMock.Verify(x => x.GetLatestMeasurementValueByTypeAsync(measurementType), Times.Once);
        _notificationDaoMock.Verify(x => x.GetLatestNotificationByTypeAsync(measurementType), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByMeasurementTypeAsync_ReturnsNotificationDto_WhenThresholdNotExceeded()
    {
        var measurementType = "Temperature";
        var latestValue = 5;
        var notification = new Notification { Threshold = 10, MeasurementType = measurementType, Message = "Message1" };

        _notificationDaoMock.Setup(x => x.GetLatestMeasurementValueByTypeAsync(measurementType))
            .ReturnsAsync(latestValue);

        _notificationDaoMock.Setup(x => x.GetLatestNotificationByTypeAsync(measurementType))
            .ReturnsAsync(notification);

        var result = await _notificationLogic.GetNotificationByMeasurementTypeAsync(measurementType);

        Assert.NotNull(result);
        Assert.Equal("Latest Temperature of 5 is lower than the threshold of 10.", result.Message);
        _notificationDaoMock.Verify(x => x.GetLatestMeasurementValueByTypeAsync(measurementType), Times.Once);
        _notificationDaoMock.Verify(x => x.GetLatestNotificationByTypeAsync(measurementType), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByMeasurementTypeAsync_ReturnsNull_WhenNoLatestValue()
    {
        var measurementType = "Temperature";

        _notificationDaoMock.Setup(x => x.GetLatestMeasurementValueByTypeAsync(measurementType))
            .ReturnsAsync((int?)null);

        var result = await _notificationLogic.GetNotificationByMeasurementTypeAsync(measurementType);

        Assert.Null(result);
        _notificationDaoMock.Verify(x => x.GetLatestMeasurementValueByTypeAsync(measurementType), Times.Once);
    }

    [Fact]
    public async Task GetNotificationByMeasurementTypeAsync_ReturnsNull_WhenNoNotificationExists()
    {
        var measurementType = "Temperature";
        var latestValue = 15;

        _notificationDaoMock.Setup(x => x.GetLatestMeasurementValueByTypeAsync(measurementType))
            .ReturnsAsync(latestValue);

        _notificationDaoMock.Setup(x => x.GetLatestNotificationByTypeAsync(measurementType))
            .ReturnsAsync((Notification)null);

        var result = await _notificationLogic.GetNotificationByMeasurementTypeAsync(measurementType);

        Assert.Null(result);
        _notificationDaoMock.Verify(x => x.GetLatestMeasurementValueByTypeAsync(measurementType), Times.Once);
        _notificationDaoMock.Verify(x => x.GetLatestNotificationByTypeAsync(measurementType), Times.Once);
    }
    
    
}