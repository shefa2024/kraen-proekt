namespace LearnConnect.API.Models;

public class LessonNotebook
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public string Content { get; set; } = string.Empty; // Markdown or JSON for rich text
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
