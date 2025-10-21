using StupidChat.Domain.Entities;

namespace StupidChat.Application.Chat.GetMessages.Models;

public class MessageModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public required MessageContent Question { get; set; }
    public required MessageContent Answer { get; set; } 
}