using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IDeviceStatusDao
{
    Task<DeviceStatus> GetDeviceStatusAsync();
    Task UpdateDeviceStatusAsync(DeviceStatus deviceStatus);
    Task ResetCommandCodeAsync();
}