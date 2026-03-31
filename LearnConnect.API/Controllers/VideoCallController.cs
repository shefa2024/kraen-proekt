using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

/// <summary>
/// Handles creating and joining WebRTC video call rooms.
/// A "room" is identified by the reservationId (or lessonId).
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VideoCallController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VideoCallController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get room info for a reservation. Returns the roomId to use with the SignalR VideoCallHub.
    /// Both the student/parent AND the teacher can call this.
    /// </summary>
    /// <summary>
    /// Get room info by reservation ID OR lesson ID.
    /// Supports both pending reservations and confirmed lessons.
    /// </summary>
    [HttpGet("room/{id}")]
    public async Task<ActionResult<VideoCallRoomDto>> GetRoom(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Try lesson first (confirmed bookings use lesson ID)
        var lesson = await _context.Lessons
            .Include(l => l.Student).ThenInclude(s => s.User)
            .Include(l => l.Teacher).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson != null)
        {
            var isTeacherL = lesson.Teacher.UserId == userId;
            var isStudentL = lesson.Student.UserId == userId;
            var isParentL  = lesson.Student.ParentUserId == userId;
            var isAdminL   = User.IsInRole("Admin");

            if (!isTeacherL && !isStudentL && !isParentL && !isAdminL)
                return Forbid();

            return Ok(new VideoCallRoomDto
            {
                RoomId     = $"lesson-{id}",
                TeacherId  = lesson.TeacherId,
                StudentId  = lesson.StudentId,
                TeacherName = $"{lesson.Teacher.User.FirstName} {lesson.Teacher.User.LastName}",
                StudentName = $"{lesson.Student.User.FirstName} {lesson.Student.User.LastName}"
            });
        }

        // Fall back to reservation ID (pending reservations)
        var reservation = await _context.Reservations
            .Include(r => r.Student).ThenInclude(s => s.User)
            .Include(r => r.Teacher).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound(new { message = "Room not found. The lesson or reservation ID is invalid." });

        var isTeacher = reservation.Teacher.UserId == userId;
        var isStudent = reservation.Student.UserId == userId;
        var isParent  = reservation.Student.ParentUserId == userId;
        var isAdmin   = User.IsInRole("Admin");

        if (!isTeacher && !isStudent && !isParent && !isAdmin)
            return Forbid();

        return Ok(new VideoCallRoomDto
        {
            RoomId      = $"lesson-{id}",
            TeacherId   = reservation.TeacherId,
            StudentId   = reservation.StudentId,
            TeacherName = $"{reservation.Teacher.User.FirstName} {reservation.Teacher.User.LastName}",
            StudentName = $"{reservation.Student.User.FirstName} {reservation.Student.User.LastName}"
        });
    }
}
