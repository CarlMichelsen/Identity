using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.User;
using Microsoft.IdentityModel.Tokens;

namespace Implementation.Map;

public static class JwtMapper
{
    public const string UserClaim = "user";
    
    public const string AccessIdClaim = "jti";
    
    public const string RefreshIdClaim = "refresh";
    
    public const string LoginIdClaim = "login";
    
    private const string EmailClaim = "email";
    
    private const string UserIdClaim = "userid";
    
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
            new(LoginIdClaim, loginId.ToString()),
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
            new(LoginIdClaim, loginId.ToString()),
            new(RefreshIdClaim, refreshId.ToString()),
            new(AccessIdClaim, accessId.ToString()),
            new(EmailClaim, authenticatedUser.Email),
            new(UserIdClaim, authenticatedUser.Id.ToString()),
            new(UserClaim, JsonSerializer.Serialize(authenticatedUser))
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