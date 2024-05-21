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

        public async Task<T> GetLatestAsync(string type)
        {
            var result = await _context.Set<T>()
                .Where(m => m.Type == type)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("No results found.");
            }

            return result;
        }
    
        public async Task<List<Measurement>> GetAllMeasurementsAsync()
        {
            return await _context.Measurements
                .OrderBy(m => m.Id)
                .ToListAsync();
        }
}