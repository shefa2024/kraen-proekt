using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.Models;

namespace LearnConnect.API.Services;

public class ReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(IServiceProvider serviceProvider, ILogger<ReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Service starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendLessonReminders();
                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when application shuts down
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Reminder Service");
                try 
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task SendLessonReminders()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var tomorrow = DateTime.UtcNow.AddDays(1);
        var searchWindow = tomorrow.AddHours(1);

        // Find lessons tomorrow that haven't been reminded
        var lessons = await context.Lessons
            .Include(l => l.Student).ThenInclude(s => s.User)
            .Include(l => l.Teacher).ThenInclude(t => t.User)
            .Where(l => l.ScheduledDateTime >= tomorrow && l.ScheduledDateTime <= searchWindow)
            .Where(l => l.Status == LessonStatus.Scheduled)
            .ToListAsync();

        foreach (var l in lessons)
        {
            _logger.LogInformation($"Sending Reminder: Lesson {l.Id} at {l.ScheduledDateTime} for Student {l.Student.User.FirstName}");
            
            // In a real app, send email/SMS/Push
            // For now, create a message in the platform
            var msg = new Message
            {
                SenderId = 1, // System / Admin
                ReceiverId = l.Student.UserId,
                Content = $"Reminder: You have a scheduled lesson with {l.Teacher.User.FirstName} tomorrow at {l.ScheduledDateTime:HH:mm}.",
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            
            context.Messages.Add(msg);
        }

        if (lessons.Any())
        {
            await context.SaveChangesAsync();
        }
    }
}
