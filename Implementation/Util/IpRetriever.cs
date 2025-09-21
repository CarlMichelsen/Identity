using Domain.Abstraction;
using Implementation.Configuration;
using Microsoft.AspNetCore.Http;

namespace Implementation.Util;

public static class IpRetriever
{
    public static Result<string> GetIp(HttpContext httpContext)
    {
        var headerIpFound = httpContext.Request.Headers.TryGetValue(
            ApplicationConstants.IdentityIPHeaderName,
            out var headerIp);
        
        var conventionalIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var ip = headerIpFound
            ? headerIp.FirstOrDefault() ?? conventionalIp
            : conventionalIp;
        
        if (string.IsNullOrWhiteSpace(ip))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Failed to get ip address from request");
        }

        return ip;
    }
}