using System.ComponentModel.DataAnnotations;

namespace LearnConnect.API.Models;

public enum PackageStatus
{
    Active,
    Completed,
    Cancelled
}

public class LessonPackage
{
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; }
    
    [Required]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    
    [Required]
    public int TotalLessons { get; set; }
    
    public int RemainingLessons { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    public decimal PricePerLesson => TotalLessons > 0 ? TotalPrice / TotalLessons : 0;
    
    public PackageStatus Status { get; set; } = PackageStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
