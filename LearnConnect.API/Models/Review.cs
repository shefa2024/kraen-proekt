namespace LearnConnect.API.Models;

public class Review
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
