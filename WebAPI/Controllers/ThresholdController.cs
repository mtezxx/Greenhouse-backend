using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class ThresholdController : ControllerBase
{
    private readonly IThresholdLogic _thresholdLogic;

    public ThresholdController(IThresholdLogic thresholdLogic)
    {
        _thresholdLogic = thresholdLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var thresholds = await _thresholdLogic.GetAsync();

        if (thresholds.Count() > 0)
        {
            return Ok(thresholds);
        }

        return NotFound("No thresholds found.");
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAsync()
    {
        var latest = await _thresholdLogic.GetLatestAsync();

        if (latest == null)
        {
            return NotFound("No latest threshold found.");
        }

        return Ok(latest);
    }

    [HttpGet("type")]
    public async Task<IActionResult> GetByTypeAsync([FromQuery] string type)
    {
        var threshold = await _thresholdLogic.GetByTypeAsync(type);

        if (threshold == null)
        {
            return NotFound("No threshold found by type"+type);
        }

        return Ok(threshold);
    }

    [HttpPost]
    public async Task<IActionResult> AddThresholdAsync([FromBody] ThresholdDto dto)
    {
        var thresholdDto = await _thresholdLogic.AddAsync(dto);

        return Created("/threshold", thresholdDto);
    }
}