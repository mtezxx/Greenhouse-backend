using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IMeasurementDao<T> where T : Measurement
{
    Task<List<T>> GetAllAsync();
    Task AddAsync(T measurement);
    Task SaveChangesAsync();
}
