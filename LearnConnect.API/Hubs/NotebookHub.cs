using Microsoft.AspNetCore.SignalR;
using LearnConnect.API.Data;
using Microsoft.EntityFrameworkCore;

namespace LearnConnect.API.Hubs;

public class NotebookHub : Hub
{
    private readonly ApplicationDbContext _context;

    public NotebookHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task JoinNotebook(int notebookId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, notebookId.ToString());
    }

    public async Task UpdateContent(int notebookId, string content)
    {
        // Update database (throttled/debounced in production, but direct for now)
        var notebook = await _context.LessonNotebooks.FindAsync(notebookId);
        if (notebook != null)
        {
            notebook.Content = content;
            notebook.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        await Clients.OthersInGroup(notebookId.ToString()).SendAsync("ReceiveContentUpdate", content);
    }
}
