using Application.DaoInterfaces;
using Domain.DTOs;
using EfcDataAccess.DAOs;

namespace Tests.EmailTests;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tests.Utils;
using EfcDataAccess;
using Xunit;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;

using System;
using System.Threading.Tasks;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Tests.Utils;

public class EmailDaoTest : DbTestBase
{
    private IEmailDao _emailDao;

    public EmailDaoTest()
    {
        TestInit();
        TestInitialize();
    }

    public void TestInitialize()
    {
        _emailDao = new EmailDao(DbContext);
    }

    [Fact]
    public async Task CreateAsync_AddsEmailSuccessfully()
    {
        var notificationEmail = new EmailNotification { Email = "test@example.com" };

        var result = await _emailDao.CreateAsync(notificationEmail);
        await DbContext.SaveChangesAsync();

        Assert.Equal("test@example.com", result.Email);
        var emailInDb = await DbContext.EmailNotifications.FirstOrDefaultAsync();
        Assert.NotNull(emailInDb);
        Assert.Equal("test@example.com", emailInDb.Email);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentNullException_WhenNotificationEmailIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _emailDao.CreateAsync(null));
    }

    [Fact]
    public async Task CreateAsync_ReplacesExistingEmail()
    {
        DbContext.EmailNotifications.Add(new EmailNotification { Email = "old@example.com" });
        await DbContext.SaveChangesAsync();

        var newNotificationEmail = new EmailNotification { Email = "new@example.com" };

        var result = await _emailDao.CreateAsync(newNotificationEmail);
        await DbContext.SaveChangesAsync();

        Assert.Equal("new@example.com", result.Email);
        var emailInDb = await DbContext.EmailNotifications.FirstOrDefaultAsync();
        Assert.NotNull(emailInDb);
        Assert.Equal("new@example.com", emailInDb.Email);
        Assert.Single(DbContext.EmailNotifications);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmailSuccessfully()
    {
        DbContext.EmailNotifications.Add(new EmailNotification { Email = "test@example.com" });
        await DbContext.SaveChangesAsync();

        var result = await _emailDao.GetAsync();

        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetAsync_ThrowsException_WhenNoEmailFound()
    {
        await Assert.ThrowsAsync<Exception>(() => _emailDao.GetAsync());
    }
    
    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenEmailIsEmpty()
    {
        var notificationEmail = new EmailNotification { Email = string.Empty };

        await Assert.ThrowsAsync<ArgumentException>(() => _emailDao.CreateAsync(notificationEmail));
    }
}
