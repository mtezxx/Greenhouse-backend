using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;

namespace Application.Logic;

public class ThresholdLogic : IThresholdLogic
{
    private readonly IThresholdDao _thresholdDao;

    public ThresholdLogic(IThresholdDao thresholdDao)
    {
        _thresholdDao = thresholdDao;
    }
    
    public async Task<List<ThresholdDto>> GetAsync()
    {
        var thresholds = await _thresholdDao.GetAsync();

        if (thresholds == null)
        {
            throw new Exception("Threshold(s) not found.");
        }
        
        List<ThresholdDto> dtos = new List<ThresholdDto>();
        
        foreach(var t in thresholds)
        {
            ThresholdDto temp = new ThresholdDto(t.Type, t.minValue, t.maxValue);
            dtos.Add(temp);
        }

        return dtos;
    }

    public async Task<ThresholdDto> GetByTypeAsync(string type)
    {
        if (type == null)
        {
            throw new Exception("Type cannot be null!");
        }
    
        var threshold = await _thresholdDao.GetByTypeAsync(type);
        if (threshold == null)
        {
            throw new Exception("Threshold not found.");
        }

        ThresholdDto dto = new ThresholdDto(threshold.Type, threshold.minValue, threshold.maxValue);
        return dto;
    }


    public async Task<ThresholdDto> GetLatestAsync()
    {
        var threshold = await _thresholdDao.GetByLatestAsync();

        if (threshold == null)
        {
            throw new Exception("No threshold found.");
        }
        
        ThresholdDto dto = new ThresholdDto(threshold.Type, threshold.minValue, threshold.maxValue);

        return dto;
    }

    private bool ValidateThresholdDto(ThresholdDto dto)
    {
        switch (dto.Type)
        {
            case "Light":
                return dto.MinValue >= 0 && dto.MaxValue <= 4095;
            case "Humidity":
                return dto.MinValue >= 0 && dto.MaxValue <= 100;
            case "Temperature":
                return dto.MinValue >= -50 && dto.MaxValue <= 60;
            default:
                return false;
        }
    }
    
    private string GetThresholdRangeErrorMessage(Threshold threshold)
    {
        switch (threshold.Type)
        {
            case "Light":
                return "Lumen value must range from 0 to 4095.";
            case "Humidity":
                return "Humidity value must range from 0 to 100.";
            case "Temperature":
                return "Temperature value must range from -50 to 60.";
            default:
                return "Invalid threshold type.";
        }
    }

    public async Task<ThresholdDto> AddAsync(ThresholdDto dto)
    {
        if (dto == null)
        {
            throw new Exception("Threshold cannot be null.");
        }
        
        if (ValidateThresholdDto(dto))
        {
            Threshold threshold = new Threshold()
            {
                Type = dto.Type,
                minValue = dto.MinValue,
                maxValue = dto.MaxValue
            };

            return await _thresholdDao.AddAsync(threshold);
        }

        else
        {
            Threshold threshold = new Threshold()
            {
                Type = dto.Type,
                minValue = dto.MinValue,
                maxValue = dto.MaxValue
            };
            throw new Exception(GetThresholdRangeErrorMessage(threshold));
        }
        
    }
}