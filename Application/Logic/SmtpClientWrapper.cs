using System.Net.Mail;
using Application.LogicInterfaces;

namespace Application.Logic;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public void Send(MailMessage message)
    {
        _smtpClient.Send(message);
    }
}
