using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.Entity;

namespace Application.Logic;

public class AuthLogic : IAuthLogic
{
    private readonly IAuthDao authDao;

    public AuthLogic(IAuthDao authDao)
    {
        this.authDao = authDao;
    }

    public async Task<User> GetUser(string username, string password)
    {
        User? user = await authDao.GetUserByUsernameAsync(username);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!user.Password.Equals(password))
        {
            throw new Exception("Password mismatch");
        }

        return user;
    }

    public async Task<User> ValidateUser(string username, string password)
    {
        User? user = await authDao.ValidateUserAsync(username, password);
        return user ?? throw new Exception("User validation failed");
    }
}
