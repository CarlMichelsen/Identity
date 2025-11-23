using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Refresh;

namespace Application.Service.OAuth;

public class CookieApplier(
    IOptionsSnapshot<AuthOptions> authOptions,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    IHostEnvironment hostEnvironment) : ICookieApplier
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
                authOptions.Value.AccessToken,
                value),
            TokenType.Refresh => SetAuthCookie(
                httpContext.Response,
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

    private Exception? SetAuthCookie(
        HttpResponse httpResponse,
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
                Domain = CookieMapper.GetCookieDomain(authOptions.Value.Self),
                Path = "/",
                Expires = expires,
                HttpOnly = true,
                IsEssential = true,
                Secure = !hostEnvironment.IsDevelopment(), // Make cookie secure in prod
                SameSite = SameSiteMode.Lax,
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