using Application.DaoInterfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class AuthDao : IAuthDao
{
    private readonly EfcContext context;

    public AuthDao(EfcContext context)
    {
        this.context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);

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
    
    public async Task<User> CreateUserAsync(User user) 
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}