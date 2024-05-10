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
    private readonly SmtpClient _smtpClient;

    public EmailLogic(IEmailDao emailDao)
    {
        _emailDao = emailDao;
        
        DotNetEnv.Env.TraversePath().Load();
        _smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("EMAIL_USERNAME"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD")),
            EnableSsl = true,
        };
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

    private void sendMail(string warning)
    {
        MailMessage message = new MailMessage
        {
            From = new MailAddress("greenhouse.notifications01@gmail.com"),
            Subject = "Threshold Warning",
            Body = @"
                <html>
                <head>
                    <style>
                        body {
                            font-size: 13px;
                        }
                        h1 {
                            color: #13910C;
                            font-size: 20px;
                            margin-bottom: 16px;
                        }
                        p {
                            margin-bottom: 10px;
                        }
                    </style>
                </head>
                <body>
                    <h1>" + warning + @"</h1>
                </body>
                </html>",
            IsBodyHtml = true
        };

        message.To.Add(_emailDao.GetAsync().Result.Email);
        _smtpClient.Send(message);
    }
    
    

    public async Task<EmailDto> GetAsync()
    {
        return await _emailDao.GetAsync();
    }

    public async Task CheckIfInRange(double temperature, double humidity, double light)
    {
        throw new NotImplementedException();
    }
}