namespace Domain.DTOs;

public class NotificationDto {
    public double Threshold { get; set; }
    public string MeasurementType { get; set; }
    public string Message { get; set; }
    public NotificationDto() {}


    public NotificationDto(double threshold, string measurementType, string message) {
        Threshold = threshold;
        MeasurementType = measurementType;
        Message = message;
    }
}