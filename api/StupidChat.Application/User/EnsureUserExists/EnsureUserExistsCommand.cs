using MediatR;

namespace StupidChat.Application.User.EnsureUserExists;

public class EnsureUserExistsCommand : IRequest<Guid>
{
    public required string UserName { get; set; }
}