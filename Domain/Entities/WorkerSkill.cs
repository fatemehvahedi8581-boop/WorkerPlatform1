namespace Domain.Entities;

public class WorkerSkill
{
    public int Id { get; set; }
    public int WorkerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageData { get; set; }

    public User Worker { get; set; } = null!;
}
