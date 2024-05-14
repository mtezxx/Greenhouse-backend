using System.Net.Mail;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Entity;
using Moq;

namespace Tests.EmailTests;

public class EmailLogicTest
{
    private Mock<IEmailDao> _emailDaoMock;
    private Mock<IThresholdDao> _thresholdDaoMock;
    private Mock<IMeasurementDao<Temperature>> _temperatureDaoMock;
    private Mock<IMeasurementDao<Humidity>> _humidityDaoMock;
    private Mock<ISmtpClient> _smtpClientMock;
    private EmailLogic _emailLogic;

    public EmailLogicTest()
    {
        Setup();
    }

    public void Setup()
    {
        _emailDaoMock = new Mock<IEmailDao>();
        _thresholdDaoMock = new Mock<IThresholdDao>();
        _temperatureDaoMock = new Mock<IMeasurementDao<Temperature>>();
        _humidityDaoMock = new Mock<IMeasurementDao<Humidity>>();
        _smtpClientMock = new Mock<ISmtpClient>();

        _emailLogic = new EmailLogic(_emailDaoMock.Object, _thresholdDaoMock.Object, _temperatureDaoMock.Object, _humidityDaoMock.Object, _smtpClientMock.Object);
    }

    [Fact]
    public async Task SendEmail_SendsEmail_WhenThresholdExceeded()
    {
        // Arrange
        _temperatureDaoMock.Setup(x => x.GetLatestAsync("Temperature")).ReturnsAsync(new Temperature { Value = 30 });
        _thresholdDaoMock.Setup(x => x.GetByTypeAsync("Temperature")).ReturnsAsync(new Threshold { minValue = 20, maxValue = 25 });

        // Act
        await _emailLogic.CheckIfInRange("Temperature");

        // Assert
        _smtpClientMock.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
    }
}