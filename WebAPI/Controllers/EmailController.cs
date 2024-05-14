using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]

public class EmailController : ControllerBase
{
    private readonly IEmailLogic _emailLogic;

    public EmailController(IEmailLogic emailLogic)
    {
        _emailLogic = emailLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var email = await _emailLogic.GetAsync();
            return Ok(email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] EmailDto email)
    {
        if (email == null)
        {
            throw new Exception("Email cannot be null.");
        }

        try
        {
            EmailDto created = await _emailLogic.CreateAsync(email);
            return Created("/email", created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }


}