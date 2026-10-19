namespace Domain.Entities;

public class JobAd
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int EmployerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User Employer { get; set; } = null!;
    public ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();
}
