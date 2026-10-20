using Application.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
}
