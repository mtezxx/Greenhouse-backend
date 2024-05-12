using Domain.DTOs;
using Domain.Entity;

namespace Application.LogicInterfaces;

public interface IMeasurementLogic
{
    Task<List<MeasurementDto>> GetAllMeasurements(string type);
    Task ProcessMeasurementData(List<Measurement> measurements);
    List<Measurement> ParseMeasurementData(string data);
}
