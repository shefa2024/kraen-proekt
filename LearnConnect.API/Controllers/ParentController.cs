using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

/// <summary>
/// Parent-specific operations: link children, view children's lessons, book for a child.
/// </summary>
[Authorize(Roles = "Parent")]
[ApiController]
[Route("api/[controller]")]
public class ParentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ParentController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // ─────────────────────────────────────────────────────────────────────────
    // CHILD MANAGEMENT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get all children linked to this parent
    /// </summary>
    [HttpGet("children")]
    public async Task<ActionResult<IEnumerable<ParentChildDto>>> GetMyChildren()
    {
        var parentUserId = GetCurrentUserId();

        var children = await _context.Students
            .Include(s => s.User)
            .Where(s => s.ParentUserId == parentUserId)
            .Select(s => new ParentChildDto
            {
                StudentId = s.Id,
                StudentName = s.StudentName ?? $"{s.User.FirstName} {s.User.LastName}",
                Email = s.User.Email,
                Level = s.Level
            })
            .ToListAsync();

        return Ok(children);
    }

    /// <summary>
    /// Link a child to this parent account by the child's email
    /// </summary>
    [HttpPost("children/link")]
    public async Task<IActionResult> LinkChild(LinkChildRequest request)
    {
        var parentUserId = GetCurrentUserId();

        // Find child user by email
        var childUser = await _context.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Email == request.ChildEmail
                && (u.Role == UserRole.Student));

        if (childUser == null)
            return NotFound(new { message = "Student account not found with that email" });

        if (childUser.Student == null)
            return BadRequest(new { message = "This account has no student profile" });

        if (childUser.Student.ParentUserId.HasValue && childUser.Student.ParentUserId != parentUserId)
            return BadRequest(new { message = "This student is already linked to another parent" });

        childUser.Student.ParentUserId = parentUserId;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Successfully linked {childUser.FirstName} {childUser.LastName} as your child." });
    }

    /// <summary>
    /// Unlink a child from this parent
    /// </summary>
    [HttpDelete("children/{studentId}")]
    public async Task<IActionResult> UnlinkChild(int studentId)
    {
        var parentUserId = GetCurrentUserId();

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);

        if (student == null)
            return NotFound(new { message = "Child not found or not linked to you" });

        student.ParentUserId = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Child unlinked successfully" });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BOOKING FOR CHILD
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Book a lesson for a specific child (student). 
    /// The parent creates the reservation on behalf of the child.
    /// </summary>
    [HttpPost("children/{studentId}/reservations")]
    public async Task<ActionResult<ReservationDto>> BookForChild(
        int studentId, CreateReservationRequest request)
    {
        var parentUserId = GetCurrentUserId();

        // Verify this child belongs to the parent
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);

        if (student == null)
            return NotFound(new { message = "Child not found or not linked to your account" });

        // Validate teacher
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId
                && t.VerificationStatus == TeacherVerificationStatus.Verified);

        if (teacher == null)
            return NotFound(new { message = "Teacher not found or not verified" });

        // Prevent double-booking
        var existing = await _context.Reservations
            .AnyAsync(r => r.StudentId == studentId
                && r.TeacherId == request.TeacherId
                && r.Status != ReservationStatus.Cancelled
                && r.RequestedDateTime.Date == request.RequestedDateTime.Date);

        if (existing)
            return BadRequest(new { message = "This child already has a booking with this teacher on that date" });

        var reservation = new Reservation
        {
            StudentId = studentId,
            TeacherId = request.TeacherId,
            RequestedDateTime = request.RequestedDateTime,
            DurationMinutes = request.DurationMinutes,
            Message = $"[Booked by parent] {request.Message}",
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(new ReservationDto
        {
            Id = reservation.Id,
            StudentId = reservation.StudentId,
            StudentName = $"{student.User.FirstName} {student.User.LastName}",
            TeacherId = reservation.TeacherId,
            TeacherName = $"{teacher.User.FirstName} {teacher.User.LastName}",
            RequestedDateTime = reservation.RequestedDateTime,
            DurationMinutes = reservation.DurationMinutes,
            Status = reservation.Status.ToString(),
            Message = reservation.Message,
            CreatedAt = reservation.CreatedAt
        });
    }

    /// <summary>
    /// Get all reservations for all children of this parent
    /// </summary>
    [HttpGet("children/reservations")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetChildrenReservations()
    {
        var parentUserId = GetCurrentUserId();

        var childIds = await _context.Students
            .Where(s => s.ParentUserId == parentUserId)
            .Select(s => s.Id)
            .ToListAsync();

        if (!childIds.Any())
            return Ok(new List<ReservationDto>());

        var reservations = await _context.Reservations
            .Include(r => r.Student).ThenInclude(s => s.User)
            .Include(r => r.Teacher).ThenInclude(t => t.User)
            .Where(r => childIds.Contains(r.StudentId))
            .OrderByDescending(r => r.RequestedDateTime)
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
