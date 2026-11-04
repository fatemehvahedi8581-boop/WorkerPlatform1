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
public class JobRequestsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public JobRequestsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] int jobAdId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { Error = "کاربر شناسایی نشد." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
            if (user == null)
                return Unauthorized(new { Error = "کاربر یافت نشد." });

            if (user.Role != "Worker")
                return StatusCode(StatusCodes.Status403Forbidden, new { Error = "کارفرما نمی‌تواند برای کارفرمایان دیگر درخواست انجام کار بدهد. فقط کارگر می‌تواند درخواست ارسال کند." });

            var jobAd = await _context.JobAds.FirstOrDefaultAsync(j => j.Id == jobAdId, cancellationToken);
            if (jobAd == null)
                return NotFound(new { Error = "آگهی مورد نظر یافت نشد." });

            var existingRequest = await _context.JobRequests
                .FirstOrDefaultAsync(r => r.JobAdId == jobAdId && r.WorkerId == userId.Value, cancellationToken);
            if (existingRequest != null)
                return BadRequest(new { Error = "شما قبلاً برای این آگهی درخواست داده‌اید." });

            var jobRequest = new JobRequest
            {
                JobAdId = jobAdId,
                WorkerId = userId.Value,
                Status = "Pending",
                AppliedAt = DateTime.Now
            };

            _context.JobRequests.Add(jobRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { Message = "درخواست با موفقیت ارسال شد.", RequestId = jobRequest.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { Error = "کاربر شناسایی نشد." });

            var normalizedStatus = status?.Trim();
            if (normalizedStatus is not ("Accepted" or "Rejected" or "Completed"))
                return BadRequest(new { Error = "وضعیت درخواست معتبر نیست." });

            var jobRequest = await _context.JobRequests
                .Include(r => r.JobAd)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (jobRequest == null)
                return NotFound(new { Error = "درخواست مورد نظر یافت نشد." });

            if (jobRequest.JobAd.EmployerId != userId.Value)
                return Forbid();

            if (normalizedStatus == "Completed" && jobRequest.Status != "Accepted")
                return BadRequest(new { Error = "فقط درخواست‌های تأییدشده قابل تکمیل هستند." });

            jobRequest.Status = normalizedStatus;
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { Message = "وضعیت درخواست با موفقیت تغییر کرد." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : null;
    }
}
