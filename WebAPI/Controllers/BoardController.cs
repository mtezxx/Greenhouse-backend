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


    public BoardController(IEncryptionService encryptionService, IMeasurementLogic measurementLogic)
    {
        _encryptionService = encryptionService;
        _measurementLogic = measurementLogic;
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
            
            // Encrypt the response with new structure
            byte commandCode = 1; // Assuming commandCode as 1 (update) for example
            byte ledStatus = 1;   // Assuming LED status as 1 (on) for example
            byte servoStatus = 1; // Assuming Servo status as 1 (open) for example
            byte[] responseData = _encryptionService.PrepareDataForResponse(boardId, timestamp, commandCode, ledStatus, servoStatus);
            byte[] responseEncrypted = _encryptionService.Encrypt(responseData);
            string responseEncryptedHex = _encryptionService.ToHexString(responseEncrypted);

            return Ok(new { encryptedResponse = responseEncryptedHex });
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
}