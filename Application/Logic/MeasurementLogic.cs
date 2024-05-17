using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;

namespace Application.Logic;

public class MeasurementLogic : IMeasurementLogic
{
    private readonly IMeasurementDao<Temperature> _temperatureDao;
    private readonly IMeasurementDao<Humidity> _humidityDao;
    private readonly IMeasurementDao<Light> _lightDao;

    public MeasurementLogic(IMeasurementDao<Temperature> temperatureDao, IMeasurementDao<Humidity> humidityDao, IMeasurementDao<Light> lightDao)
    {
        _temperatureDao = temperatureDao;
        _humidityDao = humidityDao;
        _lightDao = lightDao;
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
        else if (type.Equals("Light", StringComparison.OrdinalIgnoreCase))
        {
            var lights = await _lightDao.GetAllAsync();
            measurements.AddRange(lights);
        }
        else
        {
            throw new ArgumentException("Invalid measurement type specified");
        }

        return measurements.Select(m => new MeasurementDto { Value = m.Value, Time = m.Time, Type = m.Type }).ToList();
    }

    public async Task<MeasurementDto> GetLatestAsync(string type)
    {
        if (type == "Temperature")
        {
            Temperature temp = await _temperatureDao.GetLatestAsync("Temperature");
            MeasurementDto dto = new MeasurementDto { Value = temp.Value, Time = temp.Time, Type = temp.Type };
            return dto;
        }

        if (type == "Humidity")
        {
            Humidity hum = await _humidityDao.GetLatestAsync("Humidity");
            MeasurementDto dto = new MeasurementDto { Value = hum.Value, Time = hum.Time, Type = hum.Type };
            return dto;
        }

        if (type == "Light")
        {
            Light light = await _lightDao.GetLatestAsync("Light");
            MeasurementDto dto = new MeasurementDto { Value = light.Value, Time = light.Time, Type = light.Type };
            return dto;
        }

        else
        {
            throw new Exception("Invalid measurement type.");
        }
    }

    public async Task<MeasurementDto> AddAsync(MeasurementDto dto)
    {
        if (dto.Type == "Temperature")
        {
            Temperature temp = new Temperature { Value = dto.Value, Time = dto.Time, Type = dto.Type };
            var result = await _temperatureDao.AddAsync(temp);
            MeasurementDto dtoResult = new MeasurementDto()
            {
                Value = result.Value,
                Time = dto.Time,
                Type = dto.Type
            };
            return dtoResult;
        }
        if (dto.Type == "Humidity")
        {
            Humidity hum = new Humidity { Value = dto.Value, Time = dto.Time, Type = dto.Type };
            var result = await _humidityDao.AddAsync(hum);
            MeasurementDto dtoResult = new MeasurementDto()
            {
                Value = result.Value,
                Time = dto.Time,
                Type = dto.Type
            };
            return dtoResult;
        }
        if (dto.Type == "Light")
        {
            Light light = new Light { Value = dto.Value, Time = dto.Time, Type = dto.Type };
            var result = await _lightDao.AddAsync(light);
            MeasurementDto dtoResult = new MeasurementDto()
            {
                Value = result.Value,
                Time = dto.Time,
                Type = dto.Type
            };
            return dtoResult;
        }
        else
        {
            throw new Exception("Wrong measurement type.");
        }
    }
}


