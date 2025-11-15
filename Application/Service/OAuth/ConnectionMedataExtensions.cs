using Database.Entity;
using Microsoft.AspNetCore.Http;
using Presentation.Service.OAuth;

namespace Application.Service.OAuth;

public static class ConnectionMedataExtensions
{
    public static BaseConnectionMetadata GetConnectionMetadata(
        this HttpContext httpContext,
        TimeProvider timeProvider)
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        return new ConnectionMetadata
        {
            RemoteIpAddress = string.IsNullOrWhiteSpace(remoteIp)
                ? throw new OAuthException("Remote IP address not found")
                : remoteIp,
            RemotePort = httpContext.Connection.RemotePort,
            UserAgent = httpContext.Request.Headers.UserAgent,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
    }

    public static BaseConnectionMetadata GetConnectionMetadata(
        this IHttpContextAccessor httpContextAccessor,
        TimeProvider timeProvider)
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new OAuthException("HttpContext not found");
        return httpContext.GetConnectionMetadata(timeProvider);
    }

    private class ConnectionMetadata : BaseConnectionMetadata;
}