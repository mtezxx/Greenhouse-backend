using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Application.Logic;
using Application.DaoInterfaces;
using Domain.Entity;

public class AuthLogicTest
{
    private readonly Mock<IAuthDao> mockAuthDao;
    private readonly AuthLogic authLogic;

    public AuthLogicTest()
    {
        mockAuthDao = new Mock<IAuthDao>();
        authLogic = new AuthLogic(mockAuthDao.Object);
    }

    [Fact]
    public async Task GetUser_UserExistsAndPasswordMatches_ReturnsUser()
    {
        var user = new User { UserName = "validUser", Password = "correctPassword" };
        mockAuthDao.Setup(dao => dao.GetUserByUsernameAsync("validUser")).ReturnsAsync(user);

        var result = await authLogic.GetUser("validUser", "correctPassword");

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUser_UserNotFound_ThrowsException()
    {
        mockAuthDao.Setup(dao => dao.GetUserByUsernameAsync("invalidUser")).ReturnsAsync((User)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => authLogic.GetUser("invalidUser", "anyPassword"));
        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task GetUser_PasswordMismatch_ThrowsException()
    {
        var user = new User { UserName = "validUser", Password = "correctPassword" };
        mockAuthDao.Setup(dao => dao.GetUserByUsernameAsync("validUser")).ReturnsAsync(user);

        var exception = await Assert.ThrowsAsync<Exception>(() => authLogic.GetUser("validUser", "wrongPassword"));
        Assert.Equal("Password mismatch", exception.Message);
    }

    [Fact]
    public async Task ValidateUser_ValidUser_ReturnsUser()
    {
        var user = new User { UserName = "validUser", Password = "correctPassword" };
        mockAuthDao.Setup(dao => dao.ValidateUserAsync("validUser", "correctPassword")).ReturnsAsync(user);

        var result = await authLogic.ValidateUser("validUser", "correctPassword");

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task ValidateUser_UserValidationFailed_ThrowsException()
    {
        mockAuthDao.Setup(dao => dao.ValidateUserAsync("invalidUser", "anyPassword")).ReturnsAsync((User)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => authLogic.ValidateUser("invalidUser", "anyPassword"));
        Assert.Equal("User validation failed", exception.Message);
    }
}
