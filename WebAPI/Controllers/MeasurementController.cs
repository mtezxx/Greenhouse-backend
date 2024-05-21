using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
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

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] MeasurementDto dto)
    {
        var result = await _measurementLogic.AddAsync(dto);

        if (result == null)
        {
            return NotFound("Cannot add measurement.");
        }

        return Created("/measurement",result);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllMeasurements()
    {
        var measurements = await _measurementLogic.GetAllMeasurements();
        return Ok(measurements);
    }
}