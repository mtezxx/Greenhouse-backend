using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IoTDataController : ControllerBase
{
    private readonly IMeasurementLogic _measurementLogic;
    private readonly ICryptoLogic _cryptoLogic;

    public IoTDataController(IMeasurementLogic measurementLogic, ICryptoLogic cryptoService)
    {
        _measurementLogic = measurementLogic;
        _cryptoLogic = cryptoService;
    }
    

    [HttpPost("upload")]
    public async Task<IActionResult> ReceiveData([FromBody] string encryptedDataHex)
    {
        try
        {
            string processedDataHex = encryptedDataHex.StartsWith("/iot/") ? encryptedDataHex.Substring(5) : encryptedDataHex;
            var decryptedData = _cryptoLogic.Decrypt(processedDataHex);
            Console.WriteLine(decryptedData);
            var measurements = _measurementLogic.ParseMeasurementData(decryptedData);

            await _measurementLogic.ProcessMeasurementData(measurements);

            var responseMessage = "Data processed successfully";
            var encryptedResponse = _cryptoLogic.Encrypt(responseMessage);

            return Ok(new { message = encryptedResponse });
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
}