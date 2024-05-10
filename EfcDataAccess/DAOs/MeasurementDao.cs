using Application.DaoInterfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class MeasurementDao<T> : IMeasurementDao<T> where T : Measurement, new()
{
    private readonly EfcContext _context;

    public MeasurementDao(EfcContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T> AddAsync(T measurement)
    {
        _context.Set<T>().Add(measurement);
        await _context.SaveChangesAsync();
        return measurement;
    }
}
