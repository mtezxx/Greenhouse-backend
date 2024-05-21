using System;
using System.Threading.Tasks;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class AuthDaoTests
{
    private DbContextOptions<EfcContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<EfcContext>();
        builder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    [Fact]
    public async Task GetUserByUsernameAsync_UserExists_ReturnsUser()
    {
        var options = CreateNewContextOptions();

        using (var context = new EfcContext(options))
        {
            var dao = new AuthDao(context);
            context.Users.Add(new User { UserName = "testuser", Password = "password123" });
            context.SaveChanges();

            var result = await dao.GetUserByUsernameAsync("testuser");

            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
        }
    }

    [Fact]
    public async Task GetUserByUsernameAsync_UserDoesNotExist_ReturnsNull()
    {
        var options = CreateNewContextOptions();

        using (var context = new EfcContext(options))
        {
            var dao = new AuthDao(context);

            var result = await dao.GetUserByUsernameAsync("nonexistentuser");

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task ValidateUserAsync_ValidCredentials_ReturnsUser()
    {
        var options = CreateNewContextOptions();

        using (var context = new EfcContext(options))
        {
            var dao = new AuthDao(context);
            context.Users.Add(new User { UserName = "validuser", Password = "password123" });
            context.SaveChanges();

            var result = await dao.ValidateUserAsync("validuser", "password123");

            Assert.NotNull(result);
            Assert.Equal("validuser", result.UserName);
        }
    }

    [Fact]
    public async Task ValidateUserAsync_UserNotFound_ThrowsException()
    {
        var options = CreateNewContextOptions();

        using (var context = new EfcContext(options))
        {
            var dao = new AuthDao(context);

            var exception = await Assert.ThrowsAsync<Exception>(() => dao.ValidateUserAsync("unknown", "password123"));
            Assert.Equal("User not found", exception.Message);
        }
    }

    [Fact]
    public async Task ValidateUserAsync_InvalidPassword_ThrowsException()
    {
        var options = CreateNewContextOptions();

        using (var context = new EfcContext(options))
        {
            var dao = new AuthDao(context);
            context.Users.Add(new User { UserName = "validuser", Password = "password123" });
            context.SaveChanges();

            var exception = await Assert.ThrowsAsync<Exception>(() => dao.ValidateUserAsync("validuser", "wrongpassword"));
            Assert.Equal("Password mismatch", exception.Message);
        }
    }
}
