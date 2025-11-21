using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configuration.Options;

namespace Application.Service.OAuth;

public static class JwtCreator
{
    public static string CreateJwt(TokenConfiguration tokenConfiguration, JwtData jwtData, DateTimeOffset now)
    {
        var signingKey = tokenConfiguration.JwtSecrets.First();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expires = now
            .UtcDateTime
            .Add(tokenConfiguration.Lifetime);
        
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, jwtData.Sub),
            new("name", jwtData.Name),
            new(JwtRegisteredClaimNames.Email, jwtData.Email),
            new(JwtRegisteredClaimNames.Jti, jwtData.Jti),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString()),
            new(ClaimTypes.Role, string.Join(',', jwtData.Roles)),
            new("profile", jwtData.Small.AbsoluteUri),
            new("provider", jwtData.AuthenticationProvider),
            new("provider-id", jwtData.AuthenticationProviderId),
        ];

        if (jwtData.Medium?.AbsoluteUri is not null)
        {
            claims.Add(new Claim("profile-medium", jwtData.Medium.AbsoluteUri));
        }
        
        if (jwtData.Large?.AbsoluteUri is not null)
        {
            claims.Add(new Claim("profile-large", jwtData.Large.AbsoluteUri));
        }
        
        var token = new JwtSecurityToken(
            issuer: tokenConfiguration.JwtIssuer,
            audience: tokenConfiguration.JwtAudience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires,
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public record JwtData(
        string Sub,
        string Name,
        string Email,
        string Jti,
        List<string> Roles,
        Uri Small,
        Uri? Medium,
        Uri? Large,
        string AuthenticationProvider,
        string AuthenticationProviderId);
}