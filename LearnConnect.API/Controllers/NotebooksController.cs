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
public class NotebooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotebooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("student/{studentId}/teacher/{teacherId}")]
    public async Task<ActionResult<LessonNotebook>> GetOrCreateNotebook(int studentId, int teacherId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        // Authorization check: User must be specifically the student or the teacher
        var student = await _context.Students.FindAsync(studentId);
        var teacher = await _context.Teachers.FindAsync(teacherId);
        
        if (student == null || teacher == null) return NotFound();
        
        if (student.UserId != userId && teacher.UserId != userId) return Forbid();

        var notebook = await _context.LessonNotebooks
            .FirstOrDefaultAsync(n => n.StudentId == studentId && n.TeacherId == teacherId);

        if (notebook == null)
        {
            notebook = new LessonNotebook
            {
                StudentId = studentId,
                TeacherId = teacherId,
                Content = "# Shared Lesson Notebook\n\nStart taking notes together!",
                LastUpdatedAt = DateTime.UtcNow
            };
            _context.LessonNotebooks.Add(notebook);
            await _context.SaveChangesAsync();
        }

        return Ok(notebook);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotebook(int id, [FromBody] string content)
    {
        var notebook = await _context.LessonNotebooks.FindAsync(id);
        if (notebook == null) return NotFound();

        notebook.Content = content;
        notebook.LastUpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<LessonNotebook>> GetByLesson(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return NotFound("Lesson not found");
        return await GetOrCreateNotebook(lesson.StudentId, lesson.TeacherId);
    }
} 
