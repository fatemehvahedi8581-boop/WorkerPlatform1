using Domain.Entities;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<JobAd> JobAds { get; set; }
    public DbSet<JobRequest> JobRequests { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobAd>()
            .HasOne(j => j.Employer)
            .WithMany(u => u.JobAds)
            .HasForeignKey(j => j.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<JobRequest>()
            .HasOne(jr => jr.JobAd)
            .WithMany(j => j.JobRequests)
            .HasForeignKey(jr => jr.JobAdId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobRequest>()
            .HasOne(jr => jr.Worker)
            .WithMany(u => u.JobRequests)
            .HasForeignKey(jr => jr.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.JobRequest)
            .WithOne(jr => jr.Review)
            .HasForeignKey<Review>(r => r.JobRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
