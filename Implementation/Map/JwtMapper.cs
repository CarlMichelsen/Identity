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
        long accessId,
        string refreshJwtId,
        string accessJwtId,
        AuthenticatedUser authenticatedUser,
        JwtOptions jwtOptions)
    {
        Claim[] sharedClaims = [
            new("login", loginId.ToString()),
            new("refresh", refreshId.ToString()),
        ];
        
        Claim[] refreshClaims = [
            new("jti", refreshJwtId),
            ..sharedClaims,
        ];
        
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
        
        Claim[] accessClaims = [
            new("jti", accessJwtId),
            new("access", accessId.ToString()),
            new("userid", authenticatedUser.Id.ToString()),
            new("user", JsonSerializer.Serialize(authenticatedUser)),
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