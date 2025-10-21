using MediatR;

namespace StupidChat.Application.Chat.CreateChat;

public class CreateChatCommand : IRequest<Guid>
{
}