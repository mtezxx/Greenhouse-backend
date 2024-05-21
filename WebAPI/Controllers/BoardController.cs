using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
    private readonly IEncryptionService _encryptionService;
    private readonly IMeasurementLogic _measurementLogic;
    private readonly IDeviceStatusLogic _deviceStatusLogic;


    public BoardController(IEncryptionService encryptionService, IMeasurementLogic measurementLogic, IDeviceStatusLogic deviceStatusLogic)
    {
        _encryptionService = encryptionService;
        _measurementLogic = measurementLogic;
        _deviceStatusLogic = deviceStatusLogic;
    }


    [HttpGet("{encryptedData}")]
    public async Task<IActionResult> GetBoardData(string encryptedData)
    {
        try
        {
            byte[] encryptedBytes = _encryptionService.FromHexString(encryptedData);
            byte[] decryptedBytes = _encryptionService.Decrypt(encryptedBytes);

            var (boardId, timestamp, humidity, temperature, lux, unused, crc) = _encryptionService.ParseDataForDecryption(decryptedBytes);

            var responseMessage = $"Board ID: {boardId}, Timestamp: {timestamp}, Humidity: {humidity}, Temperature: {temperature}, Lux: {lux}, CRC: {crc:X2}";

            var temperatureMeasurement = new MeasurementDto { Value = temperature, Time = DateTime.Now, Type = "Temperature" };
            var humidityMeasurement = new MeasurementDto { Value = humidity, Time = DateTime.Now, Type = "Humidity" };
            var lightMeasurement = new MeasurementDto { Value = lux, Time = DateTime.Now, Type = "Light" };

            await _measurementLogic.AddAsync(temperatureMeasurement);
            await _measurementLogic.AddAsync(humidityMeasurement);
            await _measurementLogic.AddAsync(lightMeasurement);
            
            var deviceStatus = await _deviceStatusLogic.GetDeviceStatusAsync();

            uint currentTimestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Current time for the response
            byte[] responseData = _encryptionService.PrepareDataForResponse(boardId, currentTimestamp, deviceStatus.CommandCode, deviceStatus.LedStatus, deviceStatus.WindowStatus);
            byte[] responseEncrypted = _encryptionService.Encrypt(responseData);
            string responseEncryptedHex = _encryptionService.ToHexString(responseEncrypted);

            await _deviceStatusLogic.ResetCommandCodeAsync();

            return Ok(new { encryptedResponse = responseEncryptedHex });
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
    
    [HttpPatch("update-status")]
    public async Task<IActionResult> UpdateDeviceStatus([FromBody] DeviceStatusDto statusDto)
    {
        try
        {
            await _deviceStatusLogic.UpdateDeviceStatusAsync(statusDto.WindowStatus, statusDto.LedStatus);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
    
    
    [HttpGet("window-status")]
    public async Task<IActionResult> GetWindowStatus()
    {
        try
        {
            var deviceStatus = await _deviceStatusLogic.GetDeviceStatusAsync();
            return Ok(deviceStatus.WindowStatus);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("led-status")]
    public async Task<IActionResult> GetLedStatus()
    {
        try
        {
            var deviceStatus = await _deviceStatusLogic.GetDeviceStatusAsync();
            return Ok(deviceStatus.LedStatus);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
}