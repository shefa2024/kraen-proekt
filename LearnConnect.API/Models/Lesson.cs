namespace LearnConnect.API.Models;

public enum LessonStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public class Lesson
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int? LessonPackageId { get; set; }
    public LessonPackage? LessonPackage { get; set; }
    
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public LessonStatus Status { get; set; } = LessonStatus.Scheduled;
    public string? Notes { get; set; }
    public string? MeetingLink { get; set; }
    public decimal Price { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
