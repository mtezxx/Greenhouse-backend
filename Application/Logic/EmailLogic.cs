using System.Net;
using System.Net.Mail;
using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Exception = System.Exception;

namespace Application.Logic;


public class EmailLogic : IEmailLogic
{
    private readonly IEmailDao _emailDao;
    private readonly IThresholdDao _thresholdDao;
    private readonly IMeasurementDao<Temperature> _temperatureDao;
    private readonly IMeasurementDao<Humidity> _humidityDao;
    private readonly IMeasurementDao<Light> _lightDao;
    private readonly SmtpClient _smtpClient;

    public EmailLogic(IEmailDao emailDao, IThresholdDao thresholdDao, IMeasurementDao<Temperature> temperatureDao, IMeasurementDao<Humidity> humidityDao, IMeasurementDao<Light> lightDao,SmtpClient smtpClient)
    {
        _emailDao = emailDao;
        _thresholdDao = thresholdDao;
        _temperatureDao = temperatureDao;
        _humidityDao = humidityDao;
        _lightDao = lightDao;
        _smtpClient = smtpClient;
    }
    public async Task<EmailDto> CreateAsync(EmailDto dto)
    {
        if (dto == null)
        {
            throw new Exception("Email dto cannot be null.");
        }
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new Exception("Email address cannot be empty.");
        }

        var email = new EmailNotification()
        {
            Email = dto.Email
        };

        return await _emailDao.CreateAsync(email);
    }
    

    public async Task<EmailDto> GetAsync()
    {
        return await _emailDao.GetAsync();
    }

    public async Task CheckIfInRange(string type)
    {
        // init measurement
        Measurement measurement = null;
        
        //assign correct value to measurement
        switch (type)
        {
            case "Temperature":
                measurement = await _temperatureDao.GetLatestAsync(type);
                break;
            case "Humidity":
                measurement = await _humidityDao.GetLatestAsync(type);
                break;
            case "Light":
                measurement = await _lightDao.GetLatestAsync(type);
                break;
            default:
                throw new ArgumentException("Invalid measurement type.", nameof(type));
        }
        
        //Check if measurement is still null
        if (measurement == null)
        {
            throw new Exception("Measurement not found.");
        }

        var threshold = await _thresholdDao.GetByTypeAsync(type);
        var emailDto = await _emailDao.GetAsync();
        
        //get information for email notification
        if (measurement.Value > threshold.maxValue || measurement.Value < threshold.minValue)
        {
            try
            {
                var message = $"The {type} value {measurement.Value} exceeded the threshold.";
                _smtpClient.Send("mailtrap@demomailtrap.com", "tomi.masiar@gmail.com", "Threshold Warning", message);
            }
            catch (SmtpException ex)
            {
                // Handle or log the exception
                throw new Exception("Failed to send email.", ex);
            }
        }
    }
}