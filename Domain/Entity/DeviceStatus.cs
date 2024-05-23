namespace Domain.Entity;

public class DeviceStatus
{
    public int Id { get; set; }
    public byte WindowStatus { get; set; } = 0; // 0 for closed, 1 for open
    public byte LedStatus { get; set; } = 0;    // 0 for off, 1 for on
    public byte CommandCode { get; set; } = 0;  // 0 for no change, 1 for change
}