using Domain.Configuration;
using Microsoft.AspNetCore.Http;

namespace Implementation.Util;

public static class CookieOptionsFactory
{
    public static CookieOptions CreateOptions(FeatureFlagOptions featureFlagOptions) => new()
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Secure = !featureFlagOptions.DevelopmentLoginEnabled,
        Expires = DateTimeOffset.UtcNow.Add(ApplicationConstants.RefreshTokenLifeTime),
    };
}