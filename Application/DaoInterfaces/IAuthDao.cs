using Domain.Entity;

namespace Application.DaoInterfaces;

public interface IAuthDao
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> ValidateUserAsync(string username, string password);
    Task<User> CreateUserAsync(User user);

}