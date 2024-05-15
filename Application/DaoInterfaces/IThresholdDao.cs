using Domain.DTOs;
using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IThresholdDao
{
    Task<List<Threshold>> GetAsync();
    Task<Threshold> GetByLatestAsync();
    Task<Threshold> GetByTypeAsync(string type);
    Task<ThresholdDto> AddAsync(Threshold threshold);
}