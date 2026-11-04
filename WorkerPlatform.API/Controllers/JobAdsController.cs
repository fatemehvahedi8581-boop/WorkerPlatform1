using Application.JobAds.Commands.CreateJobAd;
using Application.JobAds.Queries.GetAllJobAds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WorkerPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobAdsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobAdsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var query = new GetAllJobAdsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateJobAdDto dto)
    {
        try
        {
            if (!User.IsInRole("Employer"))
                return StatusCode(StatusCodes.Status403Forbidden, new { Error = "کارگر اجازه ثبت آگهی ندارد. فقط کارفرما می‌تواند آگهی ثبت کند." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Error = "کاربر شناسایی نشد." });

            int employerId = int.Parse(userIdClaim);

            var command = new CreateJobAdCommand(dto.Title, dto.Description, dto.City, dto.Price, employerId);
            var jobAdId = await _mediator.Send(command);

            return Ok(new { Message = "آگهی با موفقیت ثبت شد.", JobAdId = jobAdId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
