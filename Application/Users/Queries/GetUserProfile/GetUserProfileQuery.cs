using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserProfile;

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<JobHistoryDto> JobHistory { get; set; } = new();
}

public class JobHistoryDto
{
    public int RequestId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public int? Rating { get; set; }
}

public record GetUserProfileQuery(int UserId) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    public GetUserProfileQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            throw new Exception("کاربر یافت نشد.");

        var reviews = await _context.Reviews
            .Where(r => r.JobRequest.WorkerId == user.Id)
            .ToListAsync(cancellationToken);

        double avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        var history = await _context.JobRequests
            .Include(r => r.JobAd)
            .Include(r => r.Review)
            .Where(r => user.Role == "Worker" ? r.WorkerId == user.Id : r.JobAd.EmployerId == user.Id)
            .Select(r => new JobHistoryDto
            {
                RequestId = r.Id,
                JobTitle = r.JobAd.Title,
                Status = r.Status,
                AppliedAt = r.AppliedAt,
                Rating = r.Review != null ? r.Review.Rating : null
            })
            .OrderByDescending(r => r.AppliedAt)
            .ToListAsync(cancellationToken);

        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            AverageRating = Math.Round(avgRating, 1),
            TotalReviews = reviews.Count,
            JobHistory = history
        };
    }
}
