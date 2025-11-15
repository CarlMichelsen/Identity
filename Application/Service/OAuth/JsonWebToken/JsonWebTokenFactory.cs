using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Model.Token;

namespace Application.Service.OAuth.JsonWebToken;

public class JsonWebTokenFactory(
    TimeProvider timeProvider,
    IOptionsSnapshot<AuthOptions> authOptions) : IJsonWebTokenFactory
{
    public TokenPair CreateTokenPairFromNewLoginEntity(
        LoginEntity loginEntity,
        RefreshEntityId refreshEntityId,
        AccessEntityId accessEntityId)
    {
        return new TokenPair
        {
            RefreshToken = new RefreshToken { Token = CreateJwtToken(TokenType.Refresh, loginEntity, refreshEntityId.Value.ToString()) },
            AccessToken = new AccessToken { Token = CreateJwtToken(TokenType.Access, loginEntity, accessEntityId.Value.ToString()) },
        };
    }

    private string CreateJwtToken(TokenType tokenType, LoginEntity loginEntity, string tokenId)
    {
        var now = timeProvider
            .GetUtcNow();
        
        var tokenConfiguration = tokenType switch
        {
            TokenType.Access => authOptions.Value.AccessToken,
            TokenType.Refresh => authOptions.Value.RefreshToken,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };
        
        var signingKey = tokenConfiguration.JwtSecrets.First();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        
        // exp must be parsed manually
        var expires = now
            .UtcDateTime
            .Add(tokenConfiguration.Lifetime);

        var user = loginEntity.User ?? throw new OAuthException(
            $"Unable to get user when creating token from {nameof(LoginEntity)}");

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("name", user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, tokenId),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString()),
            new(ClaimTypes.Role, string.Join(',', user.Roles)),
            new("provider", user.AuthenticationProvider),
            new("provider-id", user.AuthenticationProviderId),
        ];

        var token = new JwtSecurityToken(
            issuer: tokenConfiguration.JwtIssuer,
            audience: tokenConfiguration.JwtAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}