using Presentation.Configuration;
using Database.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Presentation.Service.OAuth;

namespace Application.Service.OAuth;

public static class ConnectionMedataExtensions
{
    public static BaseConnectionMetadata GetConnectionMetadata(
        this HttpContext httpContext,
        TimeProvider timeProvider,
        IHostEnvironment hostEnvironment)
    {
        return new ConnectionMetadata
        {
            RemoteIpAddress = GetRemoteIpAddress(httpContext.Connection, hostEnvironment),
            RemotePort = httpContext.Connection.RemotePort,
            UserAgent = httpContext.Request.Headers.UserAgent,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
    }

    public static BaseConnectionMetadata GetConnectionMetadata(
        this IHttpContextAccessor httpContextAccessor,
        TimeProvider timeProvider,
        IHostEnvironment hostEnvironment)
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new OAuthException("HttpContext not found");
        return httpContext.GetConnectionMetadata(timeProvider, hostEnvironment);
    }

    private static string GetRemoteIpAddress(ConnectionInfo connectionInfo, IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.EnvironmentName == ApplicationConstants.TestEnvironment)
        {
            return "127.0.0.1";
        }
        
        var remoteIp = connectionInfo.RemoteIpAddress?.ToString();
        return string.IsNullOrWhiteSpace(remoteIp)
            ? throw new OAuthException("Remote IP address not found")
            : remoteIp;
    }

    private class ConnectionMetadata : BaseConnectionMetadata;
}