namespace Domain.Entity;

public class Threshold
{
    public int Id { get; set; }
    public string Type { get; set; }
    public double minValue { get; set; }
    public double maxValue { get; set; }
    
    public Threshold(){}
}