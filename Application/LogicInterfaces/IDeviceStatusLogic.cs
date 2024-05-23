using Domain.Entity;

namespace Application.LogicInterfaces;

public interface IDeviceStatusLogic
{
    Task<DeviceStatus> GetDeviceStatusAsync();
    Task UpdateDeviceStatusAsync(byte windowStatus, byte ledStatus);
    Task ResetCommandCodeAsync();
}