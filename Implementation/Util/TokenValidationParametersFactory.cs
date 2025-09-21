using System.Text;
using Implementation.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Implementation.Util;

public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters AccessValidationParameters(JwtOptions jwtOptions) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Crucial for lifetime validation
        ClockSkew = TimeSpan.Zero, // Crucial for lifetime validation
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.AccessSecret)),
    };
    
    public static TokenValidationParameters HeavilyUnsafeAccessValidationParametersWithoutLifetimeValidation(JwtOptions jwtOptions) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.AccessSecret)),
    };
    
    public static TokenValidationParameters RefreshValidationParameters(JwtOptions jwtOptions) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Crucial for lifetime validation
        ClockSkew = TimeSpan.Zero, // Crucial for lifetime validation
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.RefreshSecret)),
    };
}