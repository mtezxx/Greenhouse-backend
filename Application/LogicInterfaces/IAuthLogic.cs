using Domain.Entity;

namespace Application.LogicInterfaces;

public interface IAuthLogic
{
    Task<User> GetUser(string username, string password);
    public Task<User> ValidateUser(string username, string password);

}