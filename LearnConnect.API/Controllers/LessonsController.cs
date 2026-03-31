using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using LearnConnect.API.Services;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationService _gamificationService;

    public LessonsController(ApplicationDbContext context, IGamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
    }

    [HttpGet("my-lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetMyLessons()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var query = _context.Lessons
            .Include(l => l.Teacher)
                .ThenInclude(t => t.User)
            .Include(l => l.Student)
                .ThenInclude(s => s.User)
            .Include(l => l.Subject)
            .AsQueryable();

        if (userRole == "Student")
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student != null)
            {
                query = query.Where(l => l.StudentId == student.Id);
            }
        }
        else if (userRole == "Parent")
        {
            var childIds = await _context.Students
                .Where(s => s.ParentUserId == userId)
                .Select(s => s.Id)
                .ToListAsync();
            
            query = query.Where(l => childIds.Contains(l.StudentId));
        }
        else if (userRole == "Teacher")
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher != null)
            {
                query = query.Where(l => l.TeacherId == teacher.Id);
            }
        }

        var lessons = await query
            .OrderByDescending(l => l.ScheduledDateTime)
            .ToListAsync();

        var dtos = lessons.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var lesson = await _context.Lessons
            .Include(l => l.Teacher)
                .ThenInclude(t => t.User)
            .Include(l => l.Student)
                .ThenInclude(s => s.User)
            .Include(l => l.Subject)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        // Check authorization
        if (userRole != "Admin" && 
            lesson.Student.UserId != userId && 
            lesson.Teacher.UserId != userId)
        {
            return Forbid();
        }

        var dto = MapToDto(lesson);
        return Ok(dto);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateLessonStatus(int id, [FromBody] string status)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var lesson = await _context.Lessons
            .Include(l => l.Teacher)
            .Include(l => l.Student)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        // Check authorization
        if (lesson.Teacher.UserId != userId && lesson.Student.UserId != userId)
        {
            return Forbid();
        }

        if (!Enum.TryParse<LessonStatus>(status, true, out var lessonStatus))
        {
            return BadRequest(new { message = "Invalid status" });
        }

        lesson.Status = lessonStatus;
        
        if (lessonStatus == LessonStatus.Completed)
        {
            lesson.CompletedAt = DateTime.UtcNow;
            
            // Update teacher's total lessons
            var teacher = await _context.Teachers.FindAsync(lesson.TeacherId);
            if (teacher != null)
            {
                teacher.TotalLessons++;
            }

            // Award XP to student
            await _gamificationService.AwardLessonXpAsync(lesson.StudentId);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/meeting-link")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddMeetingLink(int id, [FromBody] string meetingLink)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var lesson = await _context.Lessons
            .Include(l => l.Teacher)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        if (lesson.Teacher.UserId != userId)
        {
            return Forbid();
        }

        lesson.MeetingLink = meetingLink;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private LessonDto MapToDto(Lesson lesson)
    {
        return new LessonDto
        {
            Id = lesson.Id,
            TeacherId = lesson.TeacherId,
            TeacherName = $"{lesson.Teacher.User.FirstName} {lesson.Teacher.User.LastName}",
            StudentId = lesson.StudentId,
            StudentName = $"{lesson.Student.User.FirstName} {lesson.Student.User.LastName}",
            Subject = lesson.Subject?.Name,
            ScheduledDateTime = lesson.ScheduledDateTime,
            DurationMinutes = lesson.DurationMinutes,
            Status = lesson.Status.ToString(),
            Notes = lesson.Notes,
            MeetingLink = lesson.MeetingLink,
            Price = lesson.Price
        };
    }
}
