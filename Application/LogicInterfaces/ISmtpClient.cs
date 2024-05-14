using System.Net.Mail;

namespace Application.LogicInterfaces;

public interface ISmtpClient
{
    void Send(MailMessage message);
}