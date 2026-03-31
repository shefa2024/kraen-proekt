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
public class LessonPackagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LessonPackagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Student,Parent")]
    public async Task<ActionResult<LessonPackage>> CreatePackage(CreatePackageRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        
        if (student == null) return BadRequest(new { message = "Student profile required" });

        var teacher = await _context.Teachers.FindAsync(request.TeacherId);
        if (teacher == null) return NotFound(new { message = "Teacher not found" });

        var package = new LessonPackage
        {
            StudentId = student.Id,
            TeacherId = teacher.Id,
            SubjectId = request.SubjectId,
            TotalLessons = request.TotalLessons,
            RemainingLessons = request.TotalLessons,
            TotalPrice = teacher.HourlyRate * request.TotalLessons * 0.9m, // 10% discount for package
            Status = PackageStatus.Active
        };

        _context.LessonPackages.Add(package);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, package);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonPackage>> GetPackage(int id)
    {
        var package = await _context.LessonPackages
            .Include(p => p.Teacher).ThenInclude(t => t.User)
            .Include(p => p.Student).ThenInclude(s => s.User)
            .Include(p => p.Subject)
            .Include(p => p.Lessons)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null) return NotFound();
        return Ok(package);
    }

    [HttpGet("my-packages")]
    public async Task<ActionResult<IEnumerable<LessonPackage>>> GetMyPackages()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        var query = _context.LessonPackages
            .Include(p => p.Teacher).ThenInclude(t => t.User)
            .Include(p => p.Subject)
            .AsQueryable();

        if (role == "Student")
            query = query.Where(p => p.Student.UserId == userId);
        else if (role == "Teacher")
            query = query.Where(p => p.Teacher.UserId == userId);

        return await query.ToListAsync();
    }
}

public class CreatePackageRequest
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public int TotalLessons { get; set; }
}
