using System.Security.Claims;
using StupidChat.Application.Common;

namespace StupidChat.WebApi.Authentication;

public class CurrentHttpUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid UserId =>
        httpContextAccessor.HttpContext?.User.Identity is { IsAuthenticated: true, Name: not null } 
            ? Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
            : Guid.Empty;
}