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
    public static Result<string> CreateAccessToken(
        JwtOptions jwtOptions,
        Claim[] claims)
    {
        return GenerateJwtTokenString(
            claims,
            jwtOptions.AccessSecret,
            jwtOptions.Issuer,
            jwtOptions.Audience, 
            DateTime.UtcNow.Add(ApplicationConstants.AccessTokenLifeTime));
    }
    
    public static Result<string> CreateRefreshToken(
        JwtOptions jwtOptions,
        long loginId,
        long refreshId)
    {
        Claim[] refreshClaims = [
            new("login", loginId.ToString()),
            new("jti", refreshId.ToString()),
        ];
        
        return GenerateJwtTokenString(
            refreshClaims,
            jwtOptions.RefreshSecret,
            jwtOptions.Issuer,
            jwtOptions.Audience, 
            DateTime.UtcNow.Add(ApplicationConstants.RefreshTokenLifeTime));
    }
    
    public static Result<(string AccessToken, string RefreshToken)> GetTokenPair(
        long loginId,
        long refreshId,
        long accessId,
        AuthenticatedUser authenticatedUser,
        JwtOptions jwtOptions)
    {
        var refreshTokenResult = CreateRefreshToken(jwtOptions, loginId, refreshId);
        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.Error!;
        }
        
        Claim[] accessClaims = [
            new("login", loginId.ToString()),
            new("refresh", refreshId.ToString()),
            new("jti", accessId.ToString()),
            new("email", authenticatedUser.Email),
            new("userid", authenticatedUser.Id.ToString()),
            new("user", JsonSerializer.Serialize(authenticatedUser))
        ];
        
        var accessTokenResult = CreateAccessToken(jwtOptions, accessClaims);
        if (accessTokenResult.IsError)
        {
            return accessTokenResult.Error!;
        }

        return (
            AccessToken: accessTokenResult.Unwrap(),
            RefreshToken: refreshTokenResult.Unwrap());
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