using Application.Reviews.Commands.CreateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WorkerPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReviewsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var command = new CreateReviewCommand(dto.JobRequestId, userId, dto.Rating, dto.Comment);
            var reviewId = await _mediator.Send(command);
            return Ok(new { Message = "امتیاز با موفقیت ثبت شد.", ReviewId = reviewId });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}
