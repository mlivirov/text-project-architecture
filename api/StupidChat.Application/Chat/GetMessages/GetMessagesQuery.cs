using MediatR;
using StupidChat.Application.Chat.GetMessages.Models;
using StupidChat.Application.Common.Models;

namespace StupidChat.Application.Chat.GetMessages;

public class GetMessagesQuery : IRequest<Page<MessageModel>>
{
    public Guid ChatId { get; set; }
    public required Pagination Pagination { get; init; }
}