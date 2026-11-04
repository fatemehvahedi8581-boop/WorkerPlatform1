namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Worker";
    public string? ProfileImageData { get; set; }
    public int? BirthYear { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthDay { get; set; }
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<JobAd> JobAds { get; set; } = new List<JobAd>();
    public ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<WorkerSkill> Skills { get; set; } = new List<WorkerSkill>();
}