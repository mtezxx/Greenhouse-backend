using System.Net.Mail;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Moq;

namespace Tests.EmailTests;

public class EmailLogicTest
{
    private readonly Mock<IEmailDao> _emailDaoMock;
    private readonly Mock<IThresholdDao> _thresholdDaoMock;
    private readonly Mock<IMeasurementDao<Temperature>> _temperatureDaoMock;
    private readonly Mock<IMeasurementDao<Humidity>> _humidityDaoMock;
    private readonly Mock<SmtpClient> _smtpClientMock;
    private readonly EmailLogic _emailLogic;

    public EmailLogicTest()
    {
        _emailDaoMock = new Mock<IEmailDao>();
        _thresholdDaoMock = new Mock<IThresholdDao>();
        _temperatureDaoMock = new Mock<IMeasurementDao<Temperature>>();
        _humidityDaoMock = new Mock<IMeasurementDao<Humidity>>();
        _smtpClientMock = new Mock<SmtpClient>();

        _emailLogic = new EmailLogic(
            _emailDaoMock.Object,
            _thresholdDaoMock.Object,
            _temperatureDaoMock.Object,
            _humidityDaoMock.Object,
            _smtpClientMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenEmailDtoIsNull()
    {
        await Assert.ThrowsAsync<Exception>(() => _emailLogic.CreateAsync(null));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenEmailIsEmpty()
    {
        var emailDto = new EmailDto { Email = "" };
        await Assert.ThrowsAsync<Exception>(() => _emailLogic.CreateAsync(emailDto));
    }

    [Fact]
    public async Task CreateAsync_CreatesEmailSuccessfully()
    {
        var emailDto = new EmailDto { Email = "test@example.com" };
        var emailNotification = new EmailNotification { Email = emailDto.Email };
        var expectedDto = new EmailDto { Email = emailDto.Email };

        _emailDaoMock.Setup(x => x.CreateAsync(It.IsAny<EmailNotification>()))
            .ReturnsAsync(expectedDto);

        var result = await _emailLogic.CreateAsync(emailDto);

        Assert.Equal(expectedDto.Email, result.Email);
        _emailDaoMock.Verify(x => x.CreateAsync(It.Is<EmailNotification>(e => e.Email == emailDto.Email)), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmailSuccessfully()
    {
        var expectedDto = new EmailDto { Email = "test@example.com" };

        _emailDaoMock.Setup(x => x.GetAsync()).ReturnsAsync(expectedDto);

        var result = await _emailLogic.GetAsync();

        Assert.Equal(expectedDto.Email, result.Email);
        _emailDaoMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ThrowsException_WhenNoEmailFound()
    {
        _emailDaoMock.Setup(x => x.GetAsync()).ThrowsAsync(new Exception("No email found."));

        await Assert.ThrowsAsync<Exception>(() => _emailLogic.GetAsync());
    }
}