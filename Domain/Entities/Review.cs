namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int JobRequestId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public JobRequest JobRequest { get; set; } = null!;
}
