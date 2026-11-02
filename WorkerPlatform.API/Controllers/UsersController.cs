using Application.Users.Commands.Register;
using Application.Users.Commands.Login;
using Application.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WorkerPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var command = new RegisterUserCommand(dto.FullName, dto.PhoneNumber, dto.Password, dto.Role);
            var userId = await _mediator.Send(command);

            return Ok(new { Message = "ثبت‌نام با موفقیت انجام شد.", UserId = userId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        try
        {
            var command = new LoginUserCommand(dto.PhoneNumber, dto.Password);
            var token = await _mediator.Send(command);

            return Ok(new { Message = "ورود موفقیت‌آمیز بود.", Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{id}/profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(int id)
    {
        try
        {
            var query = new GetUserProfileQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}
