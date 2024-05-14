using Domain.DTOs;
using Domain.Entity;

namespace Application.LogicInterfaces;

public interface IThresholdLogic
{
    Task<List<ThresholdDto>> GetAsync();
    Task<ThresholdDto> GetLatestAsync();
    Task<ThresholdDto> GetByTypeAsync(string type);
    Task<ThresholdDto> AddAsync(ThresholdDto dto);
}