using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Student,Parent")]
    public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationRequest request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }
            
            var userId = int.Parse(userIdStr);
            
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null && (user.Role == UserRole.Parent || user.Role == UserRole.Student))
                {
                    student = new Student { UserId = userId };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest(new { message = "Student profile not found" });
                }
            }

            // Validate teacher
            if (!await _context.Teachers.AnyAsync(t => t.Id == request.TeacherId))
            {
                return BadRequest(new { message = "Teacher not found" });
            }

            // Check for conflicting reservations
            var requestedEndTime = request.RequestedDateTime.AddMinutes(request.DurationMinutes);
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.TeacherId == request.TeacherId &&
                               r.Status != ReservationStatus.Cancelled &&
                               r.RequestedDateTime < requestedEndTime &&
                               r.RequestedDateTime.AddMinutes((double)r.DurationMinutes) > request.RequestedDateTime);

            if (hasConflict)
            {
                return BadRequest(new { message = "This time slot is already booked by another student." });
            }

            var reservation = new Reservation
            {
                StudentId = student.Id,
                TeacherId = request.TeacherId,
                RequestedDateTime = request.RequestedDateTime,
                DurationMinutes = request.DurationMinutes,
                Message = request.Message,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var reservationDto = await GetReservationDto(reservation.Id);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, detail = ex.ToString() });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var reservation = await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Teacher)
                .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        // Check authorization
        if (userRole != "Admin" && 
            reservation.Student.UserId != userId && 
            reservation.Teacher.UserId != userId)
        {
            return Forbid();
        }

        var dto = MapToDto(reservation);
        return Ok(dto);
    }

    [HttpGet("my-reservations")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetMyReservations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var query = _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Teacher)
                .ThenInclude(t => t.User)
            .AsQueryable();

        if (userRole == "Student")
        {
            query = query.Where(r => r.Student.UserId == userId);
        }
        else if (userRole == "Parent")
        {
            var childIds = await _context.Students
                .Where(s => s.ParentUserId == userId)
                .Select(s => s.Id)
                .ToListAsync();
            
            query = query.Where(r => childIds.Contains(r.StudentId));
        }
        else if (userRole == "Teacher")
        {
            query = query.Where(r => r.Teacher.UserId == userId);
        }

        var reservations = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var dtos = reservations.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateReservationStatus(int id, UpdateReservationStatusRequest request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }
            
            var userId = int.Parse(userIdStr);
            
            var reservation = await _context.Reservations
                .Include(r => r.Teacher)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found" });
            }

            // Only teacher can confirm, both can cancel
            if (request.Status == "Confirmed" && reservation.Teacher.UserId != userId)
            {
                return Forbid();
            }

            if (!Enum.TryParse<ReservationStatus>(request.Status, true, out var status))
            {
                return BadRequest(new { message = "Invalid status" });
            }

            reservation.Status = status;
            
            if (status == ReservationStatus.Confirmed)
            {
                reservation.ConfirmedAt = DateTime.UtcNow;
                
                // Set the meeting/lesson time to 5 minutes from now as requested
                reservation.RequestedDateTime = DateTime.UtcNow.AddMinutes(5);

                // Create a lesson
                var teacher = await _context.Teachers.FindAsync(reservation.TeacherId);
                if (teacher == null)
                {
                    return BadRequest(new { message = "Associated teacher not found", teacherId = reservation.TeacherId });
                }

                var lesson = new Lesson
                {
                    TeacherId = reservation.TeacherId,
                    StudentId = reservation.StudentId,
                    ScheduledDateTime = reservation.RequestedDateTime,
                    DurationMinutes = reservation.DurationMinutes,
                    Price = teacher.HourlyRate,
                    Status = LessonStatus.Scheduled,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.Lessons.Add(lesson);
                // Assign navigation property so EF Core handles the relationship correctly
                reservation.Lesson = lesson;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new { message = "Database error while updating reservation status", error = inner, detail = dbEx.ToString() });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error while updating reservation status", error = ex.Message, detail = ex.ToString() });
        }
    }

    private async Task<ReservationDto> GetReservationDto(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Teacher)
                .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        return MapToDto(reservation!);
    }

    private ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto
        {
            Id = reservation.Id,
            StudentId = reservation.StudentId,
            StudentName = $"{reservation.Student.User.FirstName} {reservation.Student.User.LastName}",
            TeacherId = reservation.TeacherId,
            TeacherName = $"{reservation.Teacher.User.FirstName} {reservation.Teacher.User.LastName}",
            RequestedDateTime = reservation.RequestedDateTime,
            DurationMinutes = reservation.DurationMinutes,
            Status = reservation.Status.ToString(),
            Message = reservation.Message,
            CreatedAt = reservation.CreatedAt
        };
    }
}
