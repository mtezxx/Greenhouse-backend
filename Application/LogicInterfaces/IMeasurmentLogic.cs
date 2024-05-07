using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface IMeasurementLogic
{
    Task<List<MeasurementDto>> GetAllMeasurements(string type);
}
