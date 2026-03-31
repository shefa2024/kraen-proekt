using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using LearnConnect.API.Services;

namespace LearnConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly IGamificationService _gamificationService;

    public AuthController(ApplicationDbContext context, IAuthService authService, IGamificationService gamificationService)
    {
        _context = context;
        _authService = authService;
        _gamificationService = gamificationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already registered" });
        }

        // Validate role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            return BadRequest(new { message = "Invalid role" });
        }

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = userRole,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create role-specific profile
        if (userRole == UserRole.Student || userRole == UserRole.Parent)
        {
            var student = new Student { UserId = user.Id };
            _context.Students.Add(student);
        }
        else if (userRole == UserRole.Teacher)
        {
            var teacher = new Teacher 
            { 
                UserId = user.Id,
                MemberSince = DateTime.UtcNow
            };
            _context.Teachers.Add(teacher);
        }

        await _context.SaveChangesAsync();

        // Generate token
        var token = _authService.GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest(new { message = "Invalid email or password" });
        }

        if (!user.IsActive)
        {
            return BadRequest(new { message = "Account is deactivated" });
        }

        var token = _authService.GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            Token = token
        });
    }

    [HttpGet("student-stats/{studentId}")]
    public async Task<ActionResult> GetStudentStats(int studentId)
    {
        var stats = await _gamificationService.GetStudentStatsAsync(studentId);
        if (stats == null) return NotFound();
        return Ok(stats);
    }
}
