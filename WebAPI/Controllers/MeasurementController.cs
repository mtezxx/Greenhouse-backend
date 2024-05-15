using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementLogic _measurementLogic;

    public MeasurementController(IMeasurementLogic measurementLogic)
    {
        _measurementLogic = measurementLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetMeasurements([FromQuery] string type)
    {
        var measurements = await _measurementLogic.GetAllMeasurements(type);
        if (measurements.Count > 0) 
        {
            return Ok(measurements);
        }
        return NotFound($"No measurements found for type: {type}");
    }

    [HttpGet("latest+measurement")]
    public async Task<IActionResult> GetLatestAsync([FromQuery] string type)
    {
        var result = await _measurementLogic.GetLatestAsync(type);

        if (result == null)
        {
            return NotFound("No measurement found.");
        }

        return Ok(result);
    }
}