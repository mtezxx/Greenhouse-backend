using Application.DaoInterfaces;
using Application.Logic;
using Domain.Entity;
using Moq;

namespace Tests.InformationHistoryTests;

public class MeasurementLogicTest
{
    private readonly Mock<IMeasurementDao<Temperature>> _mockTemperatureDao;
    private readonly Mock<IMeasurementDao<Humidity>> _mockHumidityDao;
    private readonly MeasurementLogic _measurementLogic;

    public MeasurementLogicTest()
    {
        _mockTemperatureDao = new Mock<IMeasurementDao<Temperature>>();
        _mockHumidityDao = new Mock<IMeasurementDao<Humidity>>();
        _measurementLogic = new MeasurementLogic(_mockTemperatureDao.Object, _mockHumidityDao.Object);
    }
    [Fact]
    public async Task GetAllMeasurements_ReturnsTemperatures_WhenTypeIsTemperature()
    {
        var fakeTemperatures = new List<Temperature> { new Temperature { Value = 25.5, Time = DateTime.Now } };
        _mockTemperatureDao.Setup(m => m.GetAllAsync()).ReturnsAsync(fakeTemperatures);

        var result = await _measurementLogic.GetAllMeasurements("Temperature");

        Assert.Single(result);
        Assert.Equal(25.5, result.First().Value);
        Assert.Equal("Temperature", result.First().Type);
    }

    [Fact]
    public async Task GetAllMeasurements_ReturnsHumidities_WhenTypeIsHumidity()
    {
        var fakeHumidities = new List<Humidity> { new Humidity { Value = 50, Time = DateTime.Now } };
        _mockHumidityDao.Setup(m => m.GetAllAsync()).ReturnsAsync(fakeHumidities);

        var result = await _measurementLogic.GetAllMeasurements("Humidity");

        Assert.Single(result);
        Assert.Equal(50, result.First().Value);
        Assert.Equal("Humidity", result.First().Type);
    }

    [Fact]
    public async Task GetAllMeasurements_ThrowsArgumentException_WhenTypeIsInvalid()
    {
        var invalidType = "InvalidType";

        await Assert.ThrowsAsync<ArgumentException>(() => _measurementLogic.GetAllMeasurements(invalidType));
    }
}
