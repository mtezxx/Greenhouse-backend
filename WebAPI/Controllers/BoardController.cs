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
    private static bool _allowMeasurementUpdates = true;
    private static Timer _timer;

    [HttpGet("{encryptedData}")]
    public async Task<String> GetBoardData(string encryptedData)
    {
        try
        {
            if (_timer == null)
            {
                InitializeTimerIfNeeded();
            }
            byte[] encryptedBytes = _encryptionService.FromHexString(encryptedData);
            byte[] decryptedBytes = _encryptionService.Decrypt(encryptedBytes);

            var (boardId, timestamp, humidity, temperature, lux, unused, crc) = _encryptionService.ParseDataForDecryption(decryptedBytes);

            var responseMessage = $"Board ID: {boardId}, Timestamp: {timestamp}, Humidity: {humidity}, Temperature: {temperature}, Lux: {lux}, CRC: {crc:X2}";

            var temperatureMeasurement = new MeasurementDto { Value = temperature / 100.0, Time = DateTime.Now, Type = "Temperature" };
            var humidityMeasurement = new MeasurementDto { Value = humidity / 100.0, Time = DateTime.Now, Type = "Humidity" };
            var lightMeasurement = new MeasurementDto { Value = lux, Time = DateTime.Now, Type = "Light" };

            if (_allowMeasurementUpdates)
            {
                await _measurementLogic.AddAsync(temperatureMeasurement);
                await _measurementLogic.AddAsync(humidityMeasurement);
                await _measurementLogic.AddAsync(lightMeasurement);

                _allowMeasurementUpdates = false;
                _timer = new Timer(EnableMeasurementUpdates, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
            }

            
            var deviceStatus = await _deviceStatusLogic.GetDeviceStatusAsync();

            uint currentTimestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            byte[] responseData = _encryptionService.PrepareDataForResponse(boardId, currentTimestamp, deviceStatus.CommandCode, deviceStatus.LedStatus, deviceStatus.WindowStatus);
            byte[] responseEncrypted = _encryptionService.Encrypt(responseData);
            string responseEncryptedHex = _encryptionService.ToHexString(responseEncrypted).ToLower();
            await _deviceStatusLogic.ResetCommandCodeAsync();

            return responseEncryptedHex;
        }
        catch (Exception ex)
        {
            return "An error occurred: {ex.Message}";
        }
    }
    private void EnableMeasurementUpdates(object state)
    {
        _allowMeasurementUpdates = true;
    }
    private void InitializeTimerIfNeeded()
    {
        // Initialize the timer if needed
        _allowMeasurementUpdates = true; // Assuming measurements are allowed on start
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