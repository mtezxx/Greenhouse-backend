using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class ThresholdDao : IThresholdDao
{
    private readonly EfcContext _context;

    public ThresholdDao(EfcContext context)
    {
        _context = context;
    }
    
    public async Task<List<Threshold>> GetAsync()
    {
        return await _context.Thresholds.AsNoTracking().ToListAsync();
    }

    public async Task<Threshold> GetByLatestAsync()
    {
        var result = await _context.Thresholds.OrderByDescending(t => t.Id).FirstOrDefaultAsync();

        if (result == null)
        {
            throw new Exception("No latest threshold to be found.");
        }

        return result;
    }

    public async Task<Threshold> GetByTypeAsync(string type)
    {
        var result = await _context.Thresholds.Where(t => t.Type == type).OrderByDescending(t => t.Id).FirstOrDefaultAsync();

        if (result == null)
        {
            throw new Exception("No results.");
        }

        return result;
    }

    public async Task<ThresholdDto> AddAsync(Threshold threshold)
    {
        _context.Thresholds.Add(threshold);
        await _context.SaveChangesAsync();
        ThresholdDto dto = new ThresholdDto(threshold.Type, threshold.minValue, threshold.maxValue);

        return dto;
    }
}