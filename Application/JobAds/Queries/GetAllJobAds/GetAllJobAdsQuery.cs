using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.JobAds.Queries.GetAllJobAds;

public class JobAdDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string EmployerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public record GetAllJobAdsQuery : IRequest<List<JobAdDto>>;

public class GetAllJobAdsQueryHandler : IRequestHandler<GetAllJobAdsQuery, List<JobAdDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllJobAdsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobAdDto>> Handle(GetAllJobAdsQuery request, CancellationToken cancellationToken)
    {
        return await _context.JobAds
            .Include(j => j.Employer)
            .Select(j => new JobAdDto
            {
                Id = j.Id,
                Title = j.Title,
                City = j.City,
                Price = j.Price,
                EmployerName = j.Employer.FullName,
                CreatedAt = j.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
