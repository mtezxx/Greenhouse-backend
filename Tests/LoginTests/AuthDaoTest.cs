using Xunit;
using Microsoft.EntityFrameworkCore;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Domain.Entity;
using System;
using System.Threading.Tasks;

public class AuthDaoTests
{
    private EfcContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EfcContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
       return new EfcContext(options);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_UserExists_ReturnsUser()
    {
        using var context = CreateContext();
        var dao = new AuthDao(context);
        context.Users.Add(new User { UserName = "testuser", Password = "password123" });
        context.SaveChanges();

        var result = await dao.GetUserByUsernameAsync("testuser");

        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_UserDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext();
        var dao = new AuthDao(context);

        var result = await dao.GetUserByUsernameAsync("nonexistentuser");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateUserAsync_ValidCredentials_ReturnsUser()
    {
        using var context = CreateContext();
        var dao = new AuthDao(context);
        context.Users.Add(new User { UserName = "validuser", Password = "password123" });
        context.SaveChanges();

        var result = await dao.ValidateUserAsync("validuser", "password123");

        Assert.NotNull(result);
        Assert.Equal("validuser", result.UserName);
    }

    [Fact]
    public async Task ValidateUserAsync_UserNotFound_ThrowsException()
    {
        using var context = CreateContext();
        var dao = new AuthDao(context);

        var exception = await Assert.ThrowsAsync<Exception>(() => dao.ValidateUserAsync("unknown", "password123"));
        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task ValidateUserAsync_InvalidPassword_ThrowsException()
    {
        using var context = CreateContext();
        var dao = new AuthDao(context);
        context.Users.Add(new User { UserName = "validuser", Password = "password123" });
        context.SaveChanges();

        var exception = await Assert.ThrowsAsync<Exception>(() => dao.ValidateUserAsync("validuser", "wrongpassword"));
        Assert.Equal("Password mismatch", exception.Message);
    }
}
