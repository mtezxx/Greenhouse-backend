using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class EmailDao : IEmailDao
{
    private readonly EfcContext _context;

    public EmailDao(EfcContext context)
    {
        _context = context;
    }
    public async Task<EmailDto> CreateAsync(EmailNotification notificationEmail)
    {
        if (notificationEmail == null)
        {
            throw new ArgumentNullException(nameof(notificationEmail), "Mail data cannot be null.");
        }

        EmailNotification existingNotificationEmail = await _context.EmailNotifications.FirstOrDefaultAsync();
        if (existingNotificationEmail != null)
        {
            _context.EmailNotifications.Remove(existingNotificationEmail);
        }

        EntityEntry<EmailNotification> entity = await _context.EmailNotifications.AddAsync(notificationEmail);
        await _context.SaveChangesAsync();

        return new EmailDto()
        {
            Email = entity.Entity.Email
        };
    }


    public async Task<EmailDto> GetAsync()
    {
        var email = await _context.EmailNotifications.FirstOrDefaultAsync();

        if (email == null)
        {
            throw new Exception("No email found.");
        }

        return new EmailDto()
        {
            Email = email.Email
        };
    }
}