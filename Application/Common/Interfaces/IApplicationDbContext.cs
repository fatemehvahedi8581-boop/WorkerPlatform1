using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<JobAd> JobAds { get; set; }
    DbSet<JobRequest> JobRequests { get; set; }
    DbSet<Review> Reviews { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
