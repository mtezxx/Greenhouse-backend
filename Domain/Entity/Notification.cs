namespace Domain.Entity;

public class Notification {
    public int Id { get; set; }
    public double Threshold { get; set; }
    public string MeasurementType { get; set; }
    public string Message { get; set; }

    public Notification() {}  // EF Core requires a parameterless constructor

    public Notification(double threshold, string measurementType, string message) {
        Threshold = threshold;
        MeasurementType = measurementType;
        Message = message;
    }
}