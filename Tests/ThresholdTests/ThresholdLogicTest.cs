using Application.DaoInterfaces;
using Application.Logic;
using Domain.DTOs;
using Domain.Entity;
using Moq;
using Xunit;

namespace Tests.ThresholdTests;

public class ThresholdLogicTest
{
    private readonly Mock<IThresholdDao> _thresholdDaoMock;
    private readonly ThresholdLogic _thresholdLogic;

    public ThresholdLogicTest()
    {
        _thresholdDaoMock = new Mock<IThresholdDao>();
        _thresholdLogic = new ThresholdLogic(_thresholdDaoMock.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsListOfThresholdDtos()
    {
        var thresholds = new List<Threshold>
        {
            new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 },
            new Threshold { Type = "Humidity", minValue = 20, maxValue = 80 }
        };

        _thresholdDaoMock.Setup(x => x.GetAsync())
            .ReturnsAsync(thresholds);

        var result = await _thresholdLogic.GetAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Temperature", result[0].Type);
        Assert.Equal(0, result[0].MinValue);
        Assert.Equal(50, result[0].MaxValue);
        Assert.Equal("Humidity", result[1].Type);
        Assert.Equal(20, result[1].MinValue);
        Assert.Equal(80, result[1].MaxValue);
        _thresholdDaoMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ThrowsException_WhenThresholdsNotFound()
    {
        _thresholdDaoMock.Setup(x => x.GetAsync()).ReturnsAsync((List<Threshold>)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.GetAsync());
        Assert.Equal("Threshold(s) not found.", exception.Message);
    }

    [Fact]
    public async Task GetByTypeAsync_ReturnsThresholdDto_WhenTypeExists()
    {
        var threshold = new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 };

        _thresholdDaoMock.Setup(x => x.GetByTypeAsync("Temperature"))
            .ReturnsAsync(threshold);

        var result = await _thresholdLogic.GetByTypeAsync("Temperature");

        Assert.NotNull(result);
        Assert.Equal("Temperature", result.Type);
        Assert.Equal(0, result.MinValue);
        Assert.Equal(50, result.MaxValue);
        _thresholdDaoMock.Verify(x => x.GetByTypeAsync("Temperature"), Times.Once);
    }

    [Fact]
    public async Task GetByTypeAsync_ThrowsException_WhenTypeIsNull()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.GetByTypeAsync(null));
        Assert.Equal("Type cannot be null!", exception.Message);
    }

    [Fact]
    public async Task GetByTypeAsync_ThrowsException_WhenThresholdNotFound()
    {
        _thresholdDaoMock.Setup(x => x.GetByTypeAsync("Temperature")).ReturnsAsync((Threshold)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.GetByTypeAsync("Temperature"));
        Assert.Equal("Threshold not found.", exception.Message);
    }


    [Fact]
    public async Task GetLatestAsync_ReturnsLatestThresholdDto()
    {
        var threshold = new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 };

        _thresholdDaoMock.Setup(x => x.GetByLatestAsync())
            .ReturnsAsync(threshold);

        var result = await _thresholdLogic.GetLatestAsync();

        Assert.NotNull(result);
        Assert.Equal("Temperature", result.Type);
        Assert.Equal(0, result.MinValue);
        Assert.Equal(50, result.MaxValue);
        _thresholdDaoMock.Verify(x => x.GetByLatestAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_ThrowsException_WhenNoThresholdFound()
    {
        _thresholdDaoMock.Setup(x => x.GetByLatestAsync()).ReturnsAsync((Threshold)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.GetLatestAsync());
        Assert.Equal("No threshold found.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_AddsThresholdSuccessfully()
    {
        var thresholdDto = new ThresholdDto( "Temperature", 0, 50 );

        _thresholdDaoMock.Setup(x => x.AddAsync(It.IsAny<Threshold>()))
            .ReturnsAsync(thresholdDto);

        var result = await _thresholdLogic.AddAsync(thresholdDto);

        Assert.NotNull(result);
        Assert.Equal(thresholdDto.Type, result.Type);
        Assert.Equal(thresholdDto.MinValue, result.MinValue);
        Assert.Equal(thresholdDto.MaxValue, result.MaxValue);
        _thresholdDaoMock.Verify(x => x.AddAsync(It.Is<Threshold>(t => t.Type == thresholdDto.Type && t.minValue == thresholdDto.MinValue && t.maxValue == thresholdDto.MaxValue)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ThrowsException_WhenDtoIsNull()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.AddAsync(null));
        Assert.Equal("Threshold cannot be null.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_ThrowsException_WhenDtoIsInvalid()
    {
        var invalidDto = new ThresholdDto("Temperature", -60, 70 );

        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdLogic.AddAsync(invalidDto));
        Assert.Equal("Temperature value must range from -50 to 60.", exception.Message);
    }
}
