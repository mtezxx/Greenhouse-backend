using Domain.DTOs;
using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IEmailDao
{
    Task<EmailDto> CreateAsync(EmailNotification notificationEmail);
    Task<EmailDto> GetAsync();
}