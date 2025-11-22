using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth.JsonWebToken;

namespace Application.Service.OAuth.JsonWebToken;

public class TokenValidator(
    TimeProvider timeProvider,
    IHttpContextAccessor contextAccessor,
    IOptionsSnapshot<AuthOptions> authOptions)
    : ITokenValidator
{
    private readonly JwtSecurityTokenHandler handler = new();
    
    public (ClaimsPrincipal ClaimsPrincipal, string JwtValue)? Validate(
        TokenType tokenType,
        bool validateLifetime = true)
    {
        var tokenConfiguration = tokenType switch
        {
            TokenType.Access => authOptions.Value.AccessToken,
            TokenType.Refresh => authOptions.Value.RefreshToken,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };

        var token = this.GetJwtToken(tokenConfiguration);
        if (token is null)
        {
            return null;
        }

        var signingKeys = tokenConfiguration
                .JwtSecrets
                .Select(k => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(k)));
        foreach (var secret in signingKeys)
        {
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenConfiguration.JwtIssuer,
                    ValidAudience = tokenConfiguration.JwtAudience,
                    IssuerSigningKey = secret,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = validateLifetime,
                    ClockSkew = TimeSpan.Zero,
                    LifetimeValidator = (notBefore, expires, _, _) =>
                    {
                        var now = timeProvider.GetUtcNow().UtcDateTime;
        
                        if (notBefore.HasValue && now < notBefore.Value)
                        {
                            return false;
                        }

                        if (expires.HasValue && now > expires.Value)
                        {
                            return false;
                        }

                        return true;
                    }
                };

                return (ClaimsPrincipal : handler.ValidateToken(token, parameters, out _), JwtValue: token);
            }
            catch (SecurityTokenException)
            {
                // try next key
            }
        }

        return null;
    }

    private string? GetJwtToken(TokenConfiguration tokenConfiguration)
    {
        if (contextAccessor.HttpContext is null)
        {
            return null;
        }
        
        contextAccessor
            .HttpContext
            .Request
            .Cookies
            .TryGetValue(tokenConfiguration.CookieName, out var cookie);
        return cookie;
    }
}