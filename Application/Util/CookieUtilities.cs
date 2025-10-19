using Application.Configuration.Options;
using Microsoft.AspNetCore.Http;

namespace Application.Util;

public static class CookieUtilities
{
    public static void SetAuthCookie(
        this HttpResponse httpResponse,
        TimeProvider timeProvider,
        TokenConfiguration tokenConfiguration,
        string value)
    {
        var expires = timeProvider
            .GetUtcNow()
            .Add(tokenConfiguration.Lifetime);
        var options = new CookieOptions
        {
            Expires = expires,
            HttpOnly = true,
            IsEssential = true,
        };
        
        httpResponse.Cookies.Delete(
            tokenConfiguration.CookieName);
        httpResponse.Cookies.Append(
            tokenConfiguration.CookieName,
            value,
            options);
    }
}