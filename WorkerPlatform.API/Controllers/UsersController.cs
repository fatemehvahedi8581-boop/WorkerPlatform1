using Application.Users.Commands.Register;
using Application.Users.Commands.Login;
using Application.Users.Queries.GetUserProfile;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkerPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public UsersController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
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

    [HttpPut("{id}/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claim, out var currentUserId) || currentUserId != id)
            return Forbid();

        if (dto.BirthMonth is < 1 or > 12 || dto.BirthDay is < 1 or > 31)
            return BadRequest(new { Error = "ماه یا روز تولد معتبر نیست." });

        if (dto.BirthYear is < 1300 or > 2100)
            return BadRequest(new { Error = "سال تولد معتبر نیست." });

        if (!string.IsNullOrWhiteSpace(dto.ProfileImageData) && !IsValidImageData(dto.ProfileImageData))
            return BadRequest(new { Error = "تصویر پروفایل معتبر نیست یا حجم آن بیشتر از ۲ مگابایت است." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            return NotFound(new { Error = "کاربر یافت نشد." });

        user.BirthYear = dto.BirthYear;
        user.BirthMonth = dto.BirthMonth;
        user.BirthDay = dto.BirthDay;
        user.City = dto.City?.Trim();
        user.ProfileImageData = dto.ProfileImageData;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new { Message = "پروفایل با موفقیت به‌روزرسانی شد." });
    }

    private static bool IsValidImageData(string value)
    {
        return value.Length <= 2_800_000 &&
               (value.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("data:image/webp;base64,", StringComparison.OrdinalIgnoreCase));
    }
}

public record UpdateProfileDto(
    string? ProfileImageData,
    int? BirthYear,
    int? BirthMonth,
    int? BirthDay,
    string? City);
