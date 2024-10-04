using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Implementation.Map;

public static class JwtMapper
{
    public static Result<(string AccessToken, string RefreshToken)> GetTokenPair(
        long loginId,
        long refreshId,
        UserDto user,
        JwtOptions jwtOptions)
    {
        Claim[] sharedClaims = [
            new("login", loginId.ToString()),
            new("refresh", refreshId.ToString()),
        ];
        
        Claim[] accessClaims = [
            new("jti", Guid.NewGuid().ToString()),
            new("userid", user.Id.ToString()),
            new("user", JsonSerializer.Serialize(user)),
            ..sharedClaims,
        ];
        
        Claim[] refreshClaims = [
            new("jti", Guid.NewGuid().ToString()),
            ..sharedClaims,
        ];
        
        var accessTokenResult = GenerateJwtTokenString(
            accessClaims,
            jwtOptions.AccessSecret,
            jwtOptions.Issuer,
            jwtOptions.Audience, 
            DateTime.UtcNow.Add(ApplicationConstants.AccessTokenLifeTime));
        if (accessTokenResult.IsError)
        {
            return accessTokenResult.Error!;
        }
        
        var refreshTokenResult = GenerateJwtTokenString(
            refreshClaims,
            jwtOptions.RefreshSecret,
            jwtOptions.Issuer,
            jwtOptions.Audience, 
            DateTime.UtcNow.Add(ApplicationConstants.RefreshTokenLifeTime));
        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.Error!;
        }

        return (AccessToken: accessTokenResult.Unwrap(), RefreshToken: refreshTokenResult.Unwrap());
    }
    
    private static Result<string> GenerateJwtTokenString(
        Claim[] claims,
        string secretKey,
        string issuer,
        string audience,
        DateTime expiry)
    {
        try
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                ApplicationConstants.SecurityAlgorithm);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials));
        }
        catch (System.Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "An exception was thrown while creating jwt token",
                e);
        }
    }
}