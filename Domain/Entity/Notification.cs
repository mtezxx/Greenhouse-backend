namespace Domain.Entity;

public class Notification{
    public int Id {get;set;}
    public double Threshold {get;set;}
    public int MeasurementID {get;set;}
    public string Message {get;set;}

    public Notification(double threshold, int measurementId, string message){
        Threshold = threshold;
        MeasurementID = measurementId;
        Message = message;
    }
}