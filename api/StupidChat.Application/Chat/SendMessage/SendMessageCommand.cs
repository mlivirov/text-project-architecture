using MediatR;
using StupidChat.Application.Chat.SendMessage.Models;
using StupidChat.Domain.Entities;

namespace StupidChat.Application.Chat.SendMessage;

public class SendMessageCommand : IRequest<ResponseModel>
{
    public Guid ChatId { get; set; }
    public required MessageContent Question { get; set; }
}