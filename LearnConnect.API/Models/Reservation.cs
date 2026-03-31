namespace LearnConnect.API.Models;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public class Reservation
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    
    public DateTime RequestedDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Message { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}
