using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.JsonWebToken;

namespace Application.Service.OAuth.JsonWebToken;

public class CookieApplier(
    IOptionsSnapshot<AuthOptions> authOptions,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor) : ICookieApplier
{
    public void SetCookie(
        TokenType tokenType,
        string value)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new OAuthException("HttpContext is null");
        var ex = tokenType switch
        {
            TokenType.Access => SetAuthCookie(
                httpContext.Response,
                timeProvider,
                authOptions.Value.AccessToken,
                value),
            TokenType.Refresh => SetAuthCookie(
                httpContext.Response,
                timeProvider,
                authOptions.Value.RefreshToken,
                value),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };
        
        if (ex is not null)
        {
            throw ex;
        }
    }

    public void DeleteCookie(TokenType tokenType)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new OAuthException("HttpContext is null");
        var tokenConfiguration = tokenType switch
        {
            TokenType.Access => authOptions.Value.AccessToken,
            TokenType.Refresh => authOptions.Value.RefreshToken,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };
        
        httpContext.Response.Cookies.Delete(
            tokenConfiguration.CookieName);
    }

    private static Exception? SetAuthCookie(
        HttpResponse httpResponse,
        TimeProvider timeProvider,
        TokenConfiguration tokenConfiguration,
        string value)
    {
        try
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

            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}