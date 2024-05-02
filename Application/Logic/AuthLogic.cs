using Application.LogicInterfaces;
using Domain.Entity;

namespace Application.Logic;

public class AuthLogic: IAuthLogic
{
    private readonly IList<User> users = new List<User>
    {
        new User
        {
            UserName = "Matej",
            Password = "Password",
        },
        
    };

    public Task<User> GetUser(string username, string password)
    {
        throw new NotImplementedException();
    }
    public Task<User> ValidateUser(string username, string password)
    {
        User? existingUser = users.FirstOrDefault(u => 
            u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }

        if (!existingUser.Password.Equals(password))
        {
            throw new Exception("Password mismatch");
        }

        return Task.FromResult(existingUser);
    }
}