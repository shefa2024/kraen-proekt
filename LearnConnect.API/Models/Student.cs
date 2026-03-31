namespace LearnConnect.API.Models;

public class Student
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    // Parent-Child relationship: Parent (User with Role=Parent) linked to this student
    public int? ParentUserId { get; set; }
    public string? StudentName { get; set; } // Name for display when parent books
    
    public int ExperiencePoints { get; set; } = 0;
    public int Level { get; set; } = 1;
    public int CurrentStreak { get; set; } = 0;
    public DateTime? LastLessonDate { get; set; }

    // Navigation properties
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<StudentBadge> Badges { get; set; } = new List<StudentBadge>();
}
