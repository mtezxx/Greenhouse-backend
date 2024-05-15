using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IMeasurementDao<T> where T : Measurement
{
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T measurement);
    Task<T> GetLatestAsync(string type);
}
