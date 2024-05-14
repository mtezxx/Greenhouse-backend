using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}