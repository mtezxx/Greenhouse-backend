using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.Entity;

namespace Application.Logic;

public class DeviceStatusLogic : IDeviceStatusLogic
{
    private readonly IDeviceStatusDao _deviceStatusDao;

    public DeviceStatusLogic(IDeviceStatusDao deviceStatusDao)
    {
        _deviceStatusDao = deviceStatusDao;
    }

    public async Task<DeviceStatus> GetDeviceStatusAsync()
    {
        return await _deviceStatusDao.GetDeviceStatusAsync();
    }

    public async Task UpdateDeviceStatusAsync(byte windowStatus, byte ledStatus)
    {
        var deviceStatus = await _deviceStatusDao.GetDeviceStatusAsync();
        deviceStatus.CommandCode = (deviceStatus.WindowStatus != windowStatus || deviceStatus.LedStatus != ledStatus) ? (byte)1 : (byte)0;
        deviceStatus.WindowStatus = windowStatus;
        deviceStatus.LedStatus = ledStatus;
        await _deviceStatusDao.UpdateDeviceStatusAsync(deviceStatus);
    }
    public async Task ResetCommandCodeAsync()
    {
        await _deviceStatusDao.ResetCommandCodeAsync();
    }
}