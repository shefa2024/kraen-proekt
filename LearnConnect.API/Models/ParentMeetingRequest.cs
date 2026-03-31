namespace LearnConnect.API.Models;

public enum MeetingRequestStatus
{
    Pending,
    Accepted,
    Declined,
    Completed,
    Cancelled
}

public class ParentMeetingRequest
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public User Parent { get; set; } = null!;
    
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public DateTime RequestedDateTime { get; set; }
    public int DurationMinutes { get; set; } = 10;
    public string? Reason { get; set; }
    public MeetingRequestStatus Status { get; set; } = MeetingRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
