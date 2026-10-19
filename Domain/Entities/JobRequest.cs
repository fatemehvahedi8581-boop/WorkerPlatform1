namespace Domain.Entities;

public class JobRequest
{
    public int Id { get; set; }
    public int JobAdId { get; set; }
    public int WorkerId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime AppliedAt { get; set; } = DateTime.Now;

    public JobAd JobAd { get; set; } = null!;
    public User Worker { get; set; } = null!;
    public Review? Review { get; set; }
}
