using Implementation.Configuration;
using Microsoft.AspNetCore.Http;

namespace Implementation.Util;

public static class CookieOptionsFactory
{
    public static CookieOptions CreateOptions(OAuthOptions oAuthOptions) => new()
    {
        HttpOnly = true,
        IsEssential = true,
        Domain = oAuthOptions.Development is null ? $".{oAuthOptions.AllowedRedirectDomain}" : null,
        Path = "/",
        SameSite = SameSiteMode.Strict,
        Secure = oAuthOptions.Development is null,
        Expires = DateTimeOffset.UtcNow.Add(ApplicationConstants.RefreshTokenLifeTime),
    };
}