using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface IEmailLogic
{
    Task<EmailDto> CreateAsync(EmailDto dto);
    Task<EmailDto> GetAsync();
    Task CheckIfInRange(string type);
}