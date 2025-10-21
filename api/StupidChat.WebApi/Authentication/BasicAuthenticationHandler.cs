using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using MediatR;
using StupidChat.Application.User.EnsureUserExists;
using AccessViolationException = StupidChat.Application.Common.Exceptions.AccessViolationException;

namespace StupidChat.WebApi.Authentication;

// naive auth handler just to close the gap

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISender sender)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var creds = ExtractCredentials();
            return await Authorize(creds.username, creds.password);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }

    private (string username, string password) ExtractCredentials()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            throw new AccessViolationException("No  Authorization Header found");
        }

        var authHeader = Request.Headers.Authorization.ToString();
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            throw new AccessViolationException("Invalid auth schema");
        }

        var token = authHeader["Basic ".Length..].Trim();
        var credentialsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var credentials = credentialsString.Split(':', 2);

        if (credentials.Length != 2)
        {
            throw new AccessViolationException("Invalid Authorization Header");
        }

        var username = credentials[0];
        var password = credentials[1];
        return (username, password);
    }

    private async Task<AuthenticateResult> Authorize(string username, string password)
    {
        var userId = await sender.Send(new EnsureUserExistsCommand()
        {
            UserName = username
        });

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}