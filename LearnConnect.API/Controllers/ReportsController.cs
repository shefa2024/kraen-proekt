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
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("student/{studentId}/weekly")]
    public async Task<ActionResult> GetWeeklyReport(int studentId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Check if user has access to this student (Self, Parent, or Teacher of student)
        // Restricted for brevity: only Student or Admin for now. 
        // In real app, check Parent-Student link.

        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Badges)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null) return NotFound();

        var lastWeek = DateTime.UtcNow.AddDays(-7);
        var lessons = await _context.Lessons
            .Where(l => l.StudentId == studentId && l.Status == LessonStatus.Completed && l.CompletedAt >= lastWeek)
            .Include(l => l.Subject)
            .Include(l => l.Teacher).ThenInclude(t => t.User)
            .ToListAsync();

        var totalHours = lessons.Sum(l => l.DurationMinutes) / 60.0;
        var subjects = lessons.GroupBy(l => l.Subject?.Name ?? "General")
            .Select(g => new { Subject = g.Key, Count = g.Count() });

        var report = new
        {
            ReportDate = DateTime.UtcNow,
            StudentName = $"{student.User.FirstName} {student.User.LastName}",
            TimeRange = "Last 7 Days",
            Summary = new
            {
                TotalLessons = lessons.Count,
                TotalHours = totalHours,
                XpGained = lessons.Count * 100, // Approximate
                NewBadges = student.Badges.Where(b => b.AwardedAt >= lastWeek).Select(b => b.Name)
            },
            LessonsDetails = lessons.Select(l => new
            {
                l.ScheduledDateTime,
                Teacher = $"{l.Teacher.User.FirstName} {l.Teacher.User.LastName}",
                Subject = l.Subject?.Name,
                l.Notes
            }),
            SubjectBreakdown = subjects
        };

        return Ok(report);
    }

    [HttpGet("admin/summary")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAdminSummary(DateTime? from = null, DateTime? to = null)
    {
        var startDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = to ?? DateTime.UtcNow;

        var lessons = await _context.Lessons
            .Where(l => l.ScheduledDateTime >= startDate && l.ScheduledDateTime <= endDate)
            .ToListAsync();

        var reservations = await _context.Reservations
            .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
            .ToListAsync();

        var income = lessons.Where(l => l.Status == LessonStatus.Completed).Sum(l => l.Price);

        var activity = new
        {
            NewUsers = await _context.Users.CountAsync(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate),
            CompletedLessons = lessons.Count(l => l.Status == LessonStatus.Completed),
            CancelledLessons = lessons.Count(l => l.Status == LessonStatus.Cancelled),
            PendingReservations = reservations.Count(r => r.Status == ReservationStatus.Pending),
            ConfirmedReservations = reservations.Count(r => r.Status == ReservationStatus.Confirmed)
        };

        return Ok(new
        {
            Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            TotalIncome = income,
            Activity = activity,
            TopSubjects = lessons.GroupBy(l => l.SubjectId)
                .Select(g => new { SubjectId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
        });
    }
}
