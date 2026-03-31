using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.Models;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MeetingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-meetings")]
    public async Task<ActionResult<IEnumerable<object>>> GetMyMeetings()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var query = _context.ParentMeetingRequests
            .Include(m => m.Parent)
            .Include(m => m.Teacher).ThenInclude(t => t.User)
            .Include(m => m.Student).ThenInclude(s => s.User)
            .AsQueryable();

        if (userRole == "Parent")
        {
            query = query.Where(m => m.ParentId == userId);
        }
        else if (userRole == "Teacher")
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null) return Forbid();
            query = query.Where(m => m.TeacherId == teacher.Id);
        }
        else if (userRole == "Student")
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Ok(new List<object>());
            query = query.Where(m => m.StudentId == student.Id);
        }
        else if (userRole == "Admin")
        {
            // Admins see everything
        }
        else
        {
            return Ok(new List<object>()); // Return empty instead of 403 for unknown roles
        }

        var meetings = await query
            .OrderByDescending(m => m.RequestedDateTime)
            .Select(m => new
            {
                m.Id,
                m.ParentId,
                ParentName = $"{m.Parent.FirstName} {m.Parent.LastName}",
                m.TeacherId,
                TeacherName = $"{m.Teacher.User.FirstName} {m.Teacher.User.LastName}",
                m.StudentId,
                StudentName = $"{m.Student.User.FirstName} {m.Student.User.LastName}",
                m.RequestedDateTime,
                m.DurationMinutes,
                m.Reason,
                Status = m.Status.ToString(),
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(meetings);
    }

    [HttpPost]
    [Authorize(Roles = "Parent")]
    public async Task<ActionResult> CreateMeetingRequest([FromBody] MeetingRequestCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var meeting = new ParentMeetingRequest
        {
            ParentId = userId,
            TeacherId = dto.TeacherId,
            StudentId = dto.StudentId,
            RequestedDateTime = dto.RequestedDateTime,
            Reason = dto.Reason,
            Status = MeetingRequestStatus.Pending
        };

        _context.ParentMeetingRequests.Add(meeting);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Meeting request sent successfully", meetingId = meeting.Id });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateMeetingStatus(int id, [FromBody] string status)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var meeting = await _context.ParentMeetingRequests
            .Include(m => m.Teacher)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meeting == null) return NotFound();

        // Only teacher can accept/decline
        if (meeting.Teacher.UserId != userId) return Forbid();

        if (!Enum.TryParse<MeetingRequestStatus>(status, true, out var meetingStatus))
        {
            return BadRequest("Invalid status");
        }

        if (meetingStatus == MeetingRequestStatus.Accepted)
        {
            meeting.RequestedDateTime = DateTime.UtcNow.AddMinutes(5);
        }

        meeting.Status = meetingStatus;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class MeetingRequestCreateDto
{
    public int TeacherId { get; set; }
    public int StudentId { get; set; }
    public DateTime RequestedDateTime { get; set; }
    public string? Reason { get; set; }
}
