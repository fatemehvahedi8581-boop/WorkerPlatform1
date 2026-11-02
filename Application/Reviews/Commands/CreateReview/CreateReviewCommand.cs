using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Reviews.Commands.CreateReview;

public class CreateReviewDto
{
    public int JobRequestId { get; set; }
    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; } = string.Empty;
}

public record CreateReviewCommand(int JobRequestId, int EmployerId, int Rating, string Comment) : IRequest<int>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateReviewCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var jobRequest = await _context.JobRequests
            .Include(r => r.JobAd)
            .FirstOrDefaultAsync(r => r.Id == request.JobRequestId, cancellationToken);

        if (jobRequest == null)
            throw new Exception("درخواست مورد نظر یافت نشد.");

        if (jobRequest.JobAd.EmployerId != request.EmployerId)
            throw new Exception("شما مجاز به ثبت نظر برای این درخواست نیستید.");

        if (jobRequest.Status != "Completed" && jobRequest.Status != "Accepted")
            throw new Exception("فقط برای کارهای انجام‌شده می‌توانید امتیاز ثبت کنید.");

        var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.JobRequestId == request.JobRequestId, cancellationToken);
        if (existingReview != null)
            throw new Exception("شما قبلاً برای این کار امتیاز ثبت کرده‌اید.");

        if (request.Rating < 1 || request.Rating > 5)
            throw new Exception("امتیاز باید بین ۱ تا ۵ باشد.");

        var review = new Review
        {
            JobRequestId = request.JobRequestId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);
        return review.Id;
    }
}
