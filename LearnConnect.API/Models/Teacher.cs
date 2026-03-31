namespace LearnConnect.API.Models;

public enum TeacherVerificationStatus
{
    Pending,
    Verified,
    Rejected
}

public class Teacher
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Bio { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public string? Education { get; set; }
    public int YearsOfExperience { get; set; }
    public string? Languages { get; set; } // Comma-separated
    public string? ProfileImageUrl { get; set; }
    public double AverageRating { get; set; } = 0;
    public int TotalLessons { get; set; } = 0;
    public DateTime? MemberSince { get; set; }
    public string? ResponseTime { get; set; } = "Within 2 hours";
    
    // Verification
    public TeacherVerificationStatus VerificationStatus { get; set; } = TeacherVerificationStatus.Pending;
    public bool IsVerified => VerificationStatus == TeacherVerificationStatus.Verified;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    public string? CertificateUrl { get; set; } // Uploaded certificate/diploma
    
    // Navigation properties
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
