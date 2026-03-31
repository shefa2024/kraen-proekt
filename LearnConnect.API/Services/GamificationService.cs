using LearnConnect.API.Data;
using LearnConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnConnect.API.Services;

public interface IGamificationService
{
    Task AwardLessonXpAsync(int studentId);
    Task CheckAndAwardBadgesAsync(int studentId);
    Task<object> GetStudentStatsAsync(int studentId);
}

public class GamificationService : IGamificationService
{
    private readonly ApplicationDbContext _context;

    public GamificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AwardLessonXpAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return;

        // Base XP for lesson
        int xpGained = 100;

        // Streak Bonus
        var today = DateTime.UtcNow.Date;
        if (student.LastLessonDate.HasValue)
        {
            var lastDate = student.LastLessonDate.Value.Date;
            if (lastDate == today.AddDays(-1))
            {
                student.CurrentStreak++;
            }
            else if (lastDate < today.AddDays(-1))
            {
                student.CurrentStreak = 1;
            }
        }
        else
        {
            student.CurrentStreak = 1;
        }

        student.LastLessonDate = DateTime.UtcNow;

        // Add Streak bonus (10% extra per streak day, max 100%)
        int streakBonus = (int)(xpGained * Math.Min(student.CurrentStreak * 0.1, 1.0));
        student.ExperiencePoints += (xpGained + streakBonus);

        // Level Up logic (each level requires Level * 500 XP)
        int xpForNextLevel = student.Level * 500;
        if (student.ExperiencePoints >= xpForNextLevel)
        {
            student.Level++;
            // We don't reset XP, it's cumulative for simplicity or we could reset. Let's keep it cumulative.
        }

        await _context.SaveChangesAsync();
        await CheckAndAwardBadgesAsync(studentId);
    }

    public async Task CheckAndAwardBadgesAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Badges)
            .FirstOrDefaultAsync(s => s.Id == studentId);
            
        if (student == null) return;

        var completedLessonsCount = await _context.Lessons
            .CountAsync(l => l.StudentId == studentId && l.Status == LessonStatus.Completed);

        // Badge: Beginner (1 lesson)
        if (completedLessonsCount >= 1 && !student.Badges.Any(b => b.Name == "Beginner Scholar"))
        {
            student.Badges.Add(new StudentBadge
            {
                Name = "Beginner Scholar",
                Description = "Completed your first lesson!",
                Icon = "🌱"
            });
        }

        // Badge: Consistent (5 lessons)
        if (completedLessonsCount >= 5 && !student.Badges.Any(b => b.Name == "Consistent Learner"))
        {
            student.Badges.Add(new StudentBadge
            {
                Name = "Consistent Learner",
                Description = "Completed 5 lessons!",
                Icon = "🔥"
            });
        }

        // Badge: Subject Master (10 lessons in one subject - basic version)
        var subjectGroups = await _context.Lessons
            .Where(l => l.StudentId == studentId && l.Status == LessonStatus.Completed)
            .GroupBy(l => l.SubjectId)
            .Select(g => new { SubjectId = g.Key, Count = g.Count() })
            .ToListAsync();

        if (subjectGroups.Any(g => g.Count >= 10))
        {
            if (!student.Badges.Any(b => b.Name == "Subject Master"))
            {
                 student.Badges.Add(new StudentBadge
                {
                    Name = "Subject Master",
                    Description = "Completed 10 lessons in a single subject!",
                    Icon = "🎓"
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<object> GetStudentStatsAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Badges)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null) return null!;

        return new
        {
            student.ExperiencePoints,
            student.Level,
            student.CurrentStreak,
            NextLevelXp = student.Level * 500,
            Badges = student.Badges.Select(b => new { b.Name, b.Description, b.Icon, b.AwardedAt })
        };
    }
}
