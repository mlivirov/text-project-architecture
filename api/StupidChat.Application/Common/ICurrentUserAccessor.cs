using StupidChat.Domain.Entities;

namespace StupidChat.Application.Common;

public interface ICurrentUserAccessor
{
    public Guid UserId { get; }
}