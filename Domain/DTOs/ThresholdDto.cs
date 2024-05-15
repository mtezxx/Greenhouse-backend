namespace Domain.DTOs;

public class ThresholdDto
{
    public string Type { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    public ThresholdDto(string type, double minValue, double maxValue)
    {
        Type = type;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}