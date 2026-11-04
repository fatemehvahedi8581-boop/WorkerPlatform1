using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkerPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WorkerSkillsController : ControllerBase
{
    private static readonly string[] AllowedLevels = ["Beginner", "Intermediate", "Advanced", "Expert"];
    private readonly IApplicationDbContext _context;

    public WorkerSkillsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        if (!TryGetWorkerId(out var workerId, out var error))
            return error!;

        if (!User.IsInRole("Worker"))
            return Forbid();

        var skills = await _context.WorkerSkills
            .Where(s => s.WorkerId == workerId)
            .OrderBy(s => s.Name)
            .Select(s => new WorkerSkillDto(s.Id, s.Name, s.Level, s.Description, s.ImageData))
            .ToListAsync(cancellationToken);

        return Ok(skills);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateWorkerSkillDto dto, CancellationToken cancellationToken)
    {
        if (!TryGetWorkerId(out var workerId, out var error))
            return error!;

        if (!User.IsInRole("Worker"))
            return StatusCode(StatusCodes.Status403Forbidden, new { Error = "فقط کارگر می‌تواند مهارت‌های خود را ثبت کند." });

        var name = dto.Name?.Trim();
        var level = dto.Level?.Trim();
        var description = dto.Description?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { Error = "نام مهارت الزامی است." });

        if (level is null || !AllowedLevels.Contains(level, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { Error = "سطح مهارت معتبر نیست." });

        if (!string.IsNullOrWhiteSpace(dto.ImageData) && !IsValidImageData(dto.ImageData))
            return BadRequest(new { Error = "تصویر مهارت معتبر نیست یا حجم آن بیشتر از ۲ مگابایت است." });

        var exists = await _context.WorkerSkills.AnyAsync(
            s => s.WorkerId == workerId && s.Name == name,
            cancellationToken);
        if (exists)
            return Conflict(new { Error = "این مهارت قبلاً ثبت شده است." });

        var skill = new WorkerSkill
        {
            WorkerId = workerId,
            Name = name,
            Level = AllowedLevels.First(l => l.Equals(level, StringComparison.OrdinalIgnoreCase)),
            Description = description,
            ImageData = dto.ImageData
        };

        _context.WorkerSkills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new WorkerSkillDto(skill.Id, skill.Name, skill.Level, skill.Description, skill.ImageData));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetWorkerId(out var workerId, out var error))
            return error!;

        if (!User.IsInRole("Worker"))
            return StatusCode(StatusCodes.Status403Forbidden, new { Error = "فقط کارگر می‌تواند مهارت‌های خود را حذف کند." });

        var skill = await _context.WorkerSkills
            .FirstOrDefaultAsync(s => s.Id == id && s.WorkerId == workerId, cancellationToken);
        if (skill == null)
            return NotFound(new { Error = "مهارت یافت نشد." });

        _context.WorkerSkills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private bool TryGetWorkerId(out int workerId, out IActionResult? error)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claim, out workerId))
        {
            error = Unauthorized(new { Error = "کاربر شناسایی نشد." });
            return false;
        }

        error = null;
        return true;
    }

    private static bool IsValidImageData(string value)
    {
        return value.Length <= 2_800_000 &&
               (value.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("data:image/webp;base64,", StringComparison.OrdinalIgnoreCase));
    }
}

public record CreateWorkerSkillDto(string Name, string Level, string? Description, string? ImageData);
public record WorkerSkillDto(int Id, string Name, string Level, string Description, string? ImageData);
