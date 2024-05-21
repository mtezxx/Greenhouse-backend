using Application.DaoInterfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class DeviceStatusDao : IDeviceStatusDao
{
    private readonly EfcContext _context;

    public DeviceStatusDao(EfcContext context)
    {
        _context = context;
    }

    public async Task<DeviceStatus> GetDeviceStatusAsync()
    {
        return await _context.DeviceStatuses.FirstOrDefaultAsync() ?? new DeviceStatus();
    }

    public async Task UpdateDeviceStatusAsync(DeviceStatus deviceStatus)
    {
        var existingStatus = await _context.DeviceStatuses.FirstOrDefaultAsync();
        if (existingStatus == null)
        {
            _context.DeviceStatuses.Add(deviceStatus);
        }
        else
        {
            existingStatus.WindowStatus = deviceStatus.WindowStatus;
            existingStatus.LedStatus = deviceStatus.LedStatus;
            existingStatus.CommandCode = deviceStatus.CommandCode;
            _context.DeviceStatuses.Update(existingStatus);
        }
        await _context.SaveChangesAsync();
    }
    
    public async Task ResetCommandCodeAsync()
    {
        var existingStatus = await _context.DeviceStatuses.FirstOrDefaultAsync();
        if (existingStatus != null)
        {
            existingStatus.CommandCode = 0;
            _context.DeviceStatuses.Update(existingStatus);
            await _context.SaveChangesAsync();
        }
    }
}