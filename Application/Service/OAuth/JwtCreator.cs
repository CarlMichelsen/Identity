using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Presentation;
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
            new(JwtTokenKeys.Sub, jwtData.Sub),
            new(JwtTokenKeys.Name, jwtData.Name),
            new(JwtTokenKeys.Email, jwtData.Email),
            new(JwtTokenKeys.Jti, jwtData.Jti),
            new(JwtTokenKeys.Iat, now.ToUnixTimeSeconds().ToString()),
            new(JwtTokenKeys.Role, string.Join(',', jwtData.Roles)),
            new(JwtTokenKeys.Provider, jwtData.AuthenticationProvider),
            new(JwtTokenKeys.ProviderId, jwtData.AuthenticationProviderId),
            new(JwtTokenKeys.Profile, jwtData.Small.AbsoluteUri),
        ];

        if (jwtData.Medium?.AbsoluteUri is not null)
        {
            claims.Add(new Claim(JwtTokenKeys.ProfileMedium, jwtData.Medium.AbsoluteUri));
        }
        
        if (jwtData.Large?.AbsoluteUri is not null)
        {
            claims.Add(new Claim(JwtTokenKeys.ProfileLarge, jwtData.Large.AbsoluteUri));
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