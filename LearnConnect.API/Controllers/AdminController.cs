using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;

namespace LearnConnect.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var stats = new DashboardStatsDto
        {
            TotalStudents = await _context.Students.CountAsync(),
            TotalTeachers = await _context.Teachers.CountAsync(),
            TotalLessons = await _context.Lessons.CountAsync(),
            ActiveReservations = await _context.Reservations
                .CountAsync(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed),
            TotalRevenue = await _context.Lessons
                .Where(l => l.Status == LessonStatus.Completed)
                .SumAsync(l => l.Price),
            PendingVerifications = await _context.Teachers
                .CountAsync(t => t.VerificationStatus == TeacherVerificationStatus.Pending)
        };

        return Ok(stats);
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("users/{id}/toggle-active")]
    public async Task<IActionResult> ToggleUserActive(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TEACHER VERIFICATION
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get all teachers pending verification (or all with status filter)
    /// </summary>
    [HttpGet("verifications")]
    public async Task<ActionResult<IEnumerable<TeacherVerificationDto>>> GetVerifications(
        [FromQuery] string? status = null)
    {
        var query = _context.Teachers
            .Include(t => t.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TeacherVerificationStatus>(status, true, out var parsedStatus))
            query = query.Where(t => t.VerificationStatus == parsedStatus);
        else
            query = query.Where(t => t.VerificationStatus == TeacherVerificationStatus.Pending);

        var result = await query
            .OrderBy(t => t.MemberSince)
            .Select(t => new TeacherVerificationDto
            {
                TeacherId = t.Id,
                UserId = t.UserId,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                Email = t.User.Email,
                Bio = t.Bio,
                Education = t.Education,
                YearsOfExperience = t.YearsOfExperience,
                VerificationStatus = t.VerificationStatus.ToString(),
                CertificateUrl = t.CertificateUrl,
                VerificationNotes = t.VerificationNotes,
                VerifiedAt = t.VerifiedAt,
                MemberSince = t.MemberSince ?? DateTime.UtcNow
            })
            .ToListAsync();

        return Ok(result);
    }

    /// <summary>
    /// Approve or reject a teacher's verification request
    /// </summary>
    [HttpPut("verifications/{teacherId}")]
    public async Task<IActionResult> UpdateVerification(int teacherId, TeacherVerificationRequest request)
    {
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        if (teacher == null)
            return NotFound(new { message = "Teacher not found" });

        if (!Enum.TryParse<TeacherVerificationStatus>(request.Status, true, out var newStatus))
            return BadRequest(new { message = "Invalid status. Use: Verified or Rejected" });

        teacher.VerificationStatus = newStatus;
        teacher.VerificationNotes = request.Notes;

        if (newStatus == TeacherVerificationStatus.Verified)
            teacher.VerifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = $"Teacher {teacher.User.FirstName} {teacher.User.LastName} has been {newStatus}." });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LESSONS & RESERVATIONS
    // ─────────────────────────────────────────────────────────────────────────

    [HttpGet("lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetAllLessons()
    {
        var lessons = await _context.Lessons
            .Include(l => l.Teacher)
                .ThenInclude(t => t.User)
            .Include(l => l.Student)
                .ThenInclude(s => s.User)
            .Include(l => l.Subject)
            .OrderByDescending(l => l.ScheduledDateTime)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                TeacherId = l.TeacherId,
                TeacherName = $"{l.Teacher.User.FirstName} {l.Teacher.User.LastName}",
                StudentId = l.StudentId,
                StudentName = $"{l.Student.User.FirstName} {l.Student.User.LastName}",
                Subject = l.Subject != null ? l.Subject.Name : null,
                ScheduledDateTime = l.ScheduledDateTime,
                DurationMinutes = l.DurationMinutes,
                Status = l.Status.ToString(),
                Notes = l.Notes,
                MeetingLink = l.MeetingLink,
                Price = l.Price
            })
            .ToListAsync();

        return Ok(lessons);
    }

    [HttpGet("reservations")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAllReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Teacher)
                .ThenInclude(t => t.User)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                StudentId = r.StudentId,
                StudentName = $"{r.Student.User.FirstName} {r.Student.User.LastName}",
                TeacherId = r.TeacherId,
                TeacherName = $"{r.Teacher.User.FirstName} {r.Teacher.User.LastName}",
                RequestedDateTime = r.RequestedDateTime,
                DurationMinutes = r.DurationMinutes,
                Status = r.Status.ToString(),
                Message = r.Message,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reservations);
    }
}
