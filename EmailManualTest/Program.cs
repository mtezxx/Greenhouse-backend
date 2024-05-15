using System;
using System.Net;
using System.Net.Mail;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Entity;
using EfcDataAccess.DAOs;
using DotNetEnv;
using EfcDataAccess;

namespace EmailManualTest;

class Program
{
    static async Task Main(string[] args)
    {
        // Load environment variables
        Env.TraversePath().Load();

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD")),
            EnableSsl = true,
        };

        // Replace with your actual implementation
        EfcContext context = new EfcContext();
        IEmailDao emailDao = new EmailDao(context); 
        IThresholdDao thresholdDao = new ThresholdDao(context); 
        IMeasurementDao<Temperature> temperatureDao = new MeasurementDao<Temperature>(context); 
        IMeasurementDao<Humidity> humidityDao = new MeasurementDao<Humidity>(context); 

        IEmailLogic emailLogic = new EmailLogic(emailDao, thresholdDao, temperatureDao, humidityDao, smtpClient);

        // Trigger the email manually
        await emailLogic.CheckIfInRange("Temperature");
    }
}