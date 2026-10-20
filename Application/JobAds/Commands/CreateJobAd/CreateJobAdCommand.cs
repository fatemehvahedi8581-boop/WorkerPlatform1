using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.JobAds.Commands.CreateJobAd;

public class CreateJobAdDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public record CreateJobAdCommand(string Title, string Description, string City, decimal Price, int EmployerId) : IRequest<int>;

public class CreateJobAdCommandHandler : IRequestHandler<CreateJobAdCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateJobAdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateJobAdCommand request, CancellationToken cancellationToken)
    {
        var jobAd = new JobAd
        {
            Title = request.Title,
            Description = request.Description,
            City = request.City,
            Price = request.Price,
            EmployerId = request.EmployerId,
            CreatedAt = DateTime.Now
        };

        _context.JobAds.Add(jobAd);
        await _context.SaveChangesAsync(cancellationToken);

        return jobAd.Id;
    }
}
