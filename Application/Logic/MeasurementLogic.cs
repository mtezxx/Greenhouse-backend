using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;

namespace Application.Logic;

public class MeasurementLogic : IMeasurementLogic
{
    private readonly IMeasurementDao<Temperature> _temperatureDao;
    private readonly IMeasurementDao<Humidity> _humidityDao;

    public MeasurementLogic(IMeasurementDao<Temperature> temperatureDao, IMeasurementDao<Humidity> humidityDao)
    {
        _temperatureDao = temperatureDao;
        _humidityDao = humidityDao;
    }

    public async Task<List<MeasurementDto>> GetAllMeasurements(string type)
    {
        List<Measurement> measurements = new List<Measurement>();

        if (type.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
        {
            var temps = await _temperatureDao.GetAllAsync();
            measurements.AddRange(temps);
        }
        else if (type.Equals("Humidity", StringComparison.OrdinalIgnoreCase))
        {
            var hums = await _humidityDao.GetAllAsync();
            measurements.AddRange(hums);
        }
        else
        {
            throw new ArgumentException("Invalid measurement type specified");
        }

        return measurements.Select(m => new MeasurementDto { Value = m.Value, Time = m.Time, Type = m.Type }).ToList();
    }
}


