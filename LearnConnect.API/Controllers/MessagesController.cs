using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.DTOs;
using LearnConnect.API.Models;
using System.Security.Claims;

namespace LearnConnect.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MessagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage(SendMessageRequest request)
    {
        var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var messageDto = await GetMessageDto(message.Id);
        return Ok(messageDto);
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Get all users the current user has messaged with
        var conversations = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new
            {
                UserId = g.Key,
                LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
            })
            .ToListAsync();

        var conversationDtos = new List<ConversationDto>();
        
        foreach (var conv in conversations)
        {
            var user = await _context.Users.FindAsync(conv.UserId);
            if (user != null)
            {
                conversationDtos.Add(new ConversationDto
                {
                    UserId = conv.UserId,
                    UserName = $"{user.FirstName} {user.LastName}",
                    LastMessage = conv.LastMessage?.Content,
                    LastMessageTime = conv.LastMessage?.SentAt,
                    UnreadCount = conv.UnreadCount
                });
            }
        }

        return Ok(conversationDtos.OrderByDescending(c => c.LastMessageTime));
    }

    [HttpGet("conversation/{userId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversation(int userId)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => 
                (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // Mark messages as read
        var unreadMessages = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead);
        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            msg.ReadAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        var messageDtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
            ReceiverId = m.ReceiverId,
            ReceiverName = $"{m.Receiver.FirstName} {m.Receiver.LastName}",
            Content = m.Content,
            SentAt = m.SentAt,
            IsRead = m.IsRead
        }).ToList();

        return Ok(messageDtos);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var count = await _context.Messages
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead);

        return Ok(count);
    }

    [HttpPut("{id}/mark-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        if (message.ReceiverId != userId)
        {
            return Forbid();
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<MessageDto> GetMessageDto(int id)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == id);

        return new MessageDto
        {
            Id = message!.Id,
            SenderId = message.SenderId,
            SenderName = $"{message.Sender.FirstName} {message.Sender.LastName}",
            ReceiverId = message.ReceiverId,
            ReceiverName = $"{message.Receiver.FirstName} {message.Receiver.LastName}",
            Content = message.Content,
            SentAt = message.SentAt,
            IsRead = message.IsRead
        };
    }
}
