namespace LearnConnect.API.DTOs;

// Authentication DTOs
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Student, Teacher
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

// Teacher DTOs
public class TeacherDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string? Location { get; set; }
    public string? Education { get; set; }
    public int YearsOfExperience { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<string> Subjects { get; set; } = new();
    public string? ProfileImageUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalLessons { get; set; }
    public string? MemberSince { get; set; }
    public string? ResponseTime { get; set; }
    public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected
    public bool IsVerified { get; set; }
    public string? CertificateUrl { get; set; }
}

public class TeacherSearchRequest
{
    public string? SearchTerm { get; set; }
    public string? Subject { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public double? MinRating { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UpdateTeacherProfileRequest
{
    public string? Bio { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public string? Education { get; set; }
    public int? YearsOfExperience { get; set; }
    public List<string>? Languages { get; set; }
    public List<int>? SubjectIds { get; set; }
}

// Lesson DTOs
public class LessonDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? MeetingLink { get; set; }
    public decimal Price { get; set; }
}

public class CreateLessonRequest
{
    public int TeacherId { get; set; }
    public int StudentId { get; set; }
    public int? SubjectId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? Notes { get; set; }
}

// Reservation DTOs
public class ReservationDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime RequestedDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReservationRequest
{
    public int TeacherId { get; set; }
    public DateTime RequestedDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? Message { get; set; }
}

public class UpdateReservationStatusRequest
{
    public string Status { get; set; } = string.Empty; // Confirmed, Cancelled
}

// Review DTOs
public class ReviewDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int TeacherId { get; set; }
    public int? LessonId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

// Schedule DTOs
public class ScheduleDto
{
    public int Id { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class CreateScheduleRequest
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

// Message DTOs
public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class SendMessageRequest
{
    public int ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class ConversationDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
}

// Admin DTOs
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalLessons { get; set; }
    public int ActiveReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingVerifications { get; set; }
}

// Teacher Verification DTOs
public class TeacherVerificationRequest
{
    public string Status { get; set; } = string.Empty; // Verified, Rejected
    public string? Notes { get; set; }
}

public class TeacherVerificationDto
{
    public int TeacherId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? Education { get; set; }
    public int YearsOfExperience { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public string? CertificateUrl { get; set; }
    public string? VerificationNotes { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime MemberSince { get; set; }
}

// Parent-Child Booking DTOs
public class ParentChildDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Level { get; set; }
}

public class LinkChildRequest
{
    public string ChildEmail { get; set; } = string.Empty; // Email of child's user account
}

// Video Call DTO
public class VideoCallRoomDto
{
    public string RoomId { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public int StudentId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
}

// Payment DTOs
public class PaymentRequestDto
{
    public int TeacherId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "card";
}
