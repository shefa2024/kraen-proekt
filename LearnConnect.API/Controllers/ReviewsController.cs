using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        if (student == null)
        {
            return BadRequest(new { message = "Student profile not found" });
        }

        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { message = "Rating must be between 1 and 5" });
        }

        // Check if student has had a lesson with this teacher
        var hasLesson = await _context.Lessons
            .AnyAsync(l => l.StudentId == student.Id && 
                          l.TeacherId == request.TeacherId && 
                          l.Status == LessonStatus.Completed);

        if (!hasLesson)
        {
            return BadRequest(new { message = "You can only review teachers you've had lessons with" });
        }

        var review = new Review
        {
            StudentId = student.Id,
            TeacherId = request.TeacherId,
            LessonId = request.LessonId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        
        // Update teacher's average rating
        var teacher = await _context.Teachers
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId);

        if (teacher != null)
        {
            var allRatings = await _context.Reviews
                .Where(r => r.TeacherId == request.TeacherId)
                .Select(r => r.Rating)
                .ToListAsync();
            
            allRatings.Add(request.Rating);
            teacher.AverageRating = allRatings.Average();
        }

        await _context.SaveChangesAsync();

        var reviewDto = await GetReviewDto(review.Id);
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDto);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var reviewDto = await GetReviewDto(id);
        
        if (reviewDto == null)
        {
            return NotFound();
        }

        return Ok(reviewDto);
    }

    private async Task<ReviewDto?> GetReviewDto(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return null;
        }

        return new ReviewDto
        {
            Id = review.Id,
            StudentName = $"{review.Student.User.FirstName} {review.Student.User.LastName}",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
