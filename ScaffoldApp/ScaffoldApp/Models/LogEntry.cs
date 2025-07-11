namespace ScaffoldApp.Models;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Level { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
}