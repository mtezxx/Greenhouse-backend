namespace Domain.DTOs;

public class NotificationDto
{
    public double Threshold { get; set; }
    public int MeasurementID { get; set; }
    public string Message { get; set; }

    public NotificationDto(double threshold, int measurementId, string message)
    {
        Threshold = threshold;
        MeasurementID = measurementId;
        Message = message;
    }
}