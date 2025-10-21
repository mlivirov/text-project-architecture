using StupidChat.Domain.Entities;

namespace StupidChat.Application.Chat.SendMessage.Models;

public class ResponseModel
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public required MessageContent Answer { get; set; }
}