using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TeachersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers([FromQuery] TeacherSearchRequest request)
    {
        var query = _context.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .Include(t => t.Reviews)
            .Where(t => t.User.IsActive && t.VerificationStatus == Models.TeacherVerificationStatus.Verified)
            .AsSplitQuery()
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(t => 
                t.User.FirstName.Contains(request.SearchTerm) ||
                t.User.LastName.Contains(request.SearchTerm) ||
                t.Bio.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Subject))
        {
            query = query.Where(t => t.TeacherSubjects.Any(ts => ts.Subject.Name == request.Subject));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate <= request.MaxPrice.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(t => t.AverageRating >= request.MinRating.Value);
        }

        var totalCount = await query.CountAsync();
        
        // Ensure valid paging parameters
        if (request.Page < 1) request.Page = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        
        var teachersQuery = await query
            .OrderBy(t => t.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var teachers = teachersQuery.Select(t => new TeacherDto
        {
            Id = t.Id,
            FirstName = t.User.FirstName,
            LastName = t.User.LastName,
            Email = t.User.Email,
            Bio = t.Bio,
            HourlyRate = t.HourlyRate,
            Location = t.Location,
            Education = t.Education,
            YearsOfExperience = t.YearsOfExperience,
            Languages = !string.IsNullOrEmpty(t.Languages) 
                ? t.Languages.Split(',').ToList() 
                : new List<string>(),
            Subjects = t.TeacherSubjects.Select(ts => ts.Subject.Name).ToList(),
            ProfileImageUrl = t.ProfileImageUrl,
            AverageRating = t.AverageRating,
            TotalReviews = t.Reviews.Count,
            TotalLessons = t.TotalLessons,
            MemberSince = t.MemberSince.HasValue ? t.MemberSince.Value.ToString("MMM yyyy") : null,
            ResponseTime = t.ResponseTime,
            VerificationStatus = t.VerificationStatus.ToString(),
            IsVerified = t.VerificationStatus == Models.TeacherVerificationStatus.Verified,
            CertificateUrl = t.CertificateUrl
        }).ToList();

        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        
        return Ok(teachers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
    {
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }

        var teacherDto = new TeacherDto
        {
            Id = teacher.Id,
            FirstName = teacher.User.FirstName,
            LastName = teacher.User.LastName,
            Email = teacher.User.Email,
            Bio = teacher.Bio,
            HourlyRate = teacher.HourlyRate,
            Location = teacher.Location,
            Education = teacher.Education,
            YearsOfExperience = teacher.YearsOfExperience,
            Languages = !string.IsNullOrEmpty(teacher.Languages) 
                ? teacher.Languages.Split(',').ToList() 
                : new List<string>(),
            Subjects = teacher.TeacherSubjects.Select(ts => ts.Subject.Name).ToList(),
            ProfileImageUrl = teacher.ProfileImageUrl,
            AverageRating = teacher.AverageRating,
            TotalReviews = teacher.Reviews.Count,
            TotalLessons = teacher.TotalLessons,
            MemberSince = teacher.MemberSince.HasValue ? teacher.MemberSince.Value.ToString("MMM yyyy") : null,
            ResponseTime = teacher.ResponseTime,
            VerificationStatus = teacher.VerificationStatus.ToString(),
            IsVerified = teacher.VerificationStatus == Models.TeacherVerificationStatus.Verified,
            CertificateUrl = teacher.CertificateUrl
        };

        return Ok(teacherDto);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateTeacherProfileRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var teacher = await _context.Teachers
            .Include(t => t.TeacherSubjects)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return NotFound();
        }

        // Update fields
        if (request.Bio != null) teacher.Bio = request.Bio;
        if (request.HourlyRate.HasValue) teacher.HourlyRate = request.HourlyRate.Value;
        if (request.PhoneNumber != null) teacher.PhoneNumber = request.PhoneNumber;
        if (request.Location != null) teacher.Location = request.Location;
        if (request.Education != null) teacher.Education = request.Education;
        if (request.YearsOfExperience.HasValue) teacher.YearsOfExperience = request.YearsOfExperience.Value;
        if (request.Languages != null) teacher.Languages = string.Join(",", request.Languages);

        // Update subjects
        if (request.SubjectIds != null)
        {
            // Remove existing subjects
            _context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
            
            // Add new subjects
            foreach (var subjectId in request.SubjectIds)
            {
                teacher.TeacherSubjects.Add(new Models.TeacherSubject
                {
                    TeacherId = teacher.Id,
                    SubjectId = subjectId
                });
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetTeacherReviews(int id)
    {
        var reviews = await _context.Reviews
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Where(r => r.TeacherId == id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                StudentName = $"{r.Student.User.FirstName} {r.Student.User.LastName}",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id}/schedule")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetTeacherSchedule(int id)
    {
        var schedules = await _context.Schedules
            .Where(s => s.TeacherId == id)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek.ToString(),
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                IsAvailable = s.IsAvailable
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("my-schedule")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetMySchedule()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        if (teacher == null) return NotFound("Teacher profile not found");

        return await GetTeacherSchedule(teacher.Id);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("schedule")]
    public async Task<IActionResult> UpdateSchedule([FromBody] List<ScheduleDto> requests)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        if (teacher == null) return NotFound("Teacher profile not found");

        var existingSchedules = await _context.Schedules.Where(s => s.TeacherId == teacher.Id).ToListAsync();

        foreach (var req in requests)
        {
            if (!Enum.TryParse<DayOfWeek>(req.DayOfWeek, true, out var day)) continue;
            if (!TimeSpan.TryParse(req.StartTime, out var startTime)) continue;
            if (!TimeSpan.TryParse(req.EndTime, out var endTime)) continue;

            var sched = existingSchedules.FirstOrDefault(s => s.DayOfWeek == day);
            if (sched != null)
            {
                sched.StartTime = startTime;
                sched.EndTime = endTime;
                sched.IsAvailable = req.IsAvailable;
            }
            else
            {
                _context.Schedules.Add(new Models.Schedule
                {
                    TeacherId = teacher.Id,
                    DayOfWeek = day,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = req.IsAvailable
                });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost("verify")]
    public async Task<IActionResult> SubmitVerification([FromBody] string certificateUrl)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        if (teacher == null) return NotFound();

        teacher.CertificateUrl = certificateUrl;
        teacher.VerificationStatus = Models.TeacherVerificationStatus.Pending;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Verification request submitted successfully" });
    }
}
