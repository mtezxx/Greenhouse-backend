namespace Domain.Entity;

public abstract class Measurement
{
    public int Id { get; set; }
    public double Value { get; set; }
    private DateTime _time = DateTime.Now;  // Initialize with the current time
    public DateTime Time
    {
        get => _time;
        set => _time = value;
    }
    public string Type { get; set; }
}