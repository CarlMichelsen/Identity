using System.Security.Claims;

namespace Presentation.Service.OAuth.JsonWebToken;

public interface ITokenValidator
{
    public (ClaimsPrincipal ClaimsPrincipal, string JwtValue)? Validate(
        TokenType tokenType,
        bool validateLifetime = true);
}