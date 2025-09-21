using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Database;
using Domain.Abstraction;
using Domain.Dto;
using Implementation.Configuration;
using Implementation.Map;
using Implementation.Util;
using Interface.Repository;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Implementation.Service;

public class UserRefreshService(
    IOptions<OAuthOptions> oAuthOptions,
    IOptions<IdentityCookieOptions> cookieOptions,
    IOptions<JwtOptions> jwtOptions,
    IUserRefreshRepository userRefreshRepository,
    IHttpContextAccessor httpContextAccessor) : IUserRefreshService
{
    public async Task<Result<ServiceResponse>> Refresh()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No httpContext found");
        }

        var principalPairResult = this.GetAccessAndRefreshPrincipal(httpContext);
        if (principalPairResult.IsError)
        {
            if (principalPairResult.Error!.InnerException is not null)
            {
                return principalPairResult.Error!;
            }

            return new ServiceResponse(ApplicationConstants.UnauthorizedErrorMessage);
        }

        var principalPair = principalPairResult.Unwrap();
        var refreshIdClaim = principalPair.RefreshPrincipal.Claims
            .FirstOrDefault(c => c.Type == "jti")?.Value;
        if (!long.TryParse(refreshIdClaim, out var refreshId))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No refreshId claim found");
        }

        var clientInfoResult = GetClientInfo(httpContext);
        if (clientInfoResult.IsError)
        {
            return clientInfoResult.Error!;
        }
        
        var accessRefreshPairResult = await userRefreshRepository.Refresh(clientInfoResult.Unwrap(), refreshId);
        if (accessRefreshPairResult.IsError)
        {
            return accessRefreshPairResult.Error!;
        }

        var recordPair = accessRefreshPairResult.Unwrap();
        var loginIdClaim = principalPair.RefreshPrincipal.Claims
            .FirstOrDefault(c => c.Type == "login")?.Value;
        if (!long.TryParse(loginIdClaim, out var loginId))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No loginId claim found");
        }

        var appendCookieOptions = CookieOptionsFactory.CreateOptions(oAuthOptions.Value);

        if (recordPair.NewRefreshRecord is not null)
        {
            var refreshJwtTokenStringResult = JwtMapper.CreateRefreshToken(
                jwtOptions.Value,
                loginId,
                recordPair.NewRefreshRecord.Id);
            if (refreshJwtTokenStringResult.IsError)
            {
                return refreshJwtTokenStringResult.Error!;
            }
            
            httpContext.Response.Cookies.Append(
                cookieOptions.Value.RefreshCookieName,
                refreshJwtTokenStringResult.Unwrap(),
                appendCookieOptions);
        }

        var accessClaims = TransformAccessClaims(
            principalPair.AccessPrincipal,
            recordPair.NewAccessRecord.Id,
            recordPair.NewRefreshRecord?.Id ?? refreshId);
        var accessJwtTokenStringResult = JwtMapper.CreateAccessToken(
            jwtOptions.Value, 
            accessClaims);
        if (accessJwtTokenStringResult.IsError)
        {
            return accessJwtTokenStringResult.Error!;
        }
        
        httpContext.Response.Cookies.Append(
            cookieOptions.Value.AccessCookieName,
            accessJwtTokenStringResult.Unwrap(),
            appendCookieOptions);

        return new ServiceResponse();
    }

    private static Claim[] TransformAccessClaims(ClaimsPrincipal accessClaimsPrincipal, long accessId, long refreshId)
    {
        List<string> purgeClaimTypes =
        [
            "jti",
            "exp",
            "refresh",
        ];
        
        var claims = accessClaimsPrincipal.Claims
            .Where(c => !purgeClaimTypes.Contains(c.Type))
            .ToList();
        
        claims.Add(new Claim("jti", accessId.ToString()));
        claims.Add(new Claim("refresh", refreshId.ToString()));

        return [.. claims];
    }
    
    private static Result<ClaimsPrincipal> GetJwtTokenFromCookieClaims(
        HttpContext httpContext,
        JwtSecurityTokenHandler handler,
        TokenValidationParameters validationParameters,
        string cookieName)
    {
        try
        {
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var cookie))
            {
                return new ResultError(ResultErrorType.NotFound, "No cookie");
            }

            if (string.IsNullOrWhiteSpace(cookie))
            {
                return new ResultError(ResultErrorType.NotFound, "No cookie");
            }
            
            var claimsPrincipal = handler.ValidateToken(
                cookie,
                validationParameters,
                out _);
            
            return claimsPrincipal;
        }
        catch (SecurityTokenExpiredException)
        {
            // The token is expired (should only apply to refresh token in this case)
            return new ResultError(
                ResultErrorType.NotFound,
                ApplicationConstants.UnauthorizedErrorMessage);
        }
        catch (Exception e)
        {
            // Other token validation errors
            return new ResultError(
                ResultErrorType.Exception,
                "Failed to get claims from cookie jwt token",
                e);
        }
    }

    private static Result<IClientInfo> GetClientInfo(HttpContext httpContext)
    {
        var ipResult = IpRetriever.GetIp(httpContext);

        // Get the User-Agent string from the request headers
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No userAgent found");
        }

        return new ClientInfoRecord(Ip: ipResult.Unwrap(), UserAgent: userAgent);
    }

    private Result<(ClaimsPrincipal AccessPrincipal, ClaimsPrincipal RefreshPrincipal)> GetAccessAndRefreshPrincipal(
        HttpContext httpContext)
    {
        var handler = new JwtSecurityTokenHandler();
        var unsafeAccessTokenValidationParameters = TokenValidationParametersFactory
            .HeavilyUnsafeAccessValidationParametersWithoutLifetimeValidation(jwtOptions.Value);
        var invalidAccessClaimsResult = GetJwtTokenFromCookieClaims(
            httpContext,
            handler,
            unsafeAccessTokenValidationParameters,
            cookieOptions.Value.AccessCookieName);
        if (invalidAccessClaimsResult.IsError)
        {
            return invalidAccessClaimsResult.Error!;
        }
        
        var refreshTokenValidationParameters = TokenValidationParametersFactory
            .RefreshValidationParameters(jwtOptions.Value);
        var refreshTokenClaimsResult = GetJwtTokenFromCookieClaims(
            httpContext,
            handler,
            refreshTokenValidationParameters,
            cookieOptions.Value.RefreshCookieName);
        if (refreshTokenClaimsResult.IsError)
        {
            return refreshTokenClaimsResult.Error!;
        }

        return (
            AccessPrincipal: invalidAccessClaimsResult.Unwrap(),
            RefreshPrincipal: refreshTokenClaimsResult.Unwrap());
    }
    
    private record ClientInfoRecord(string Ip, string UserAgent) : IClientInfo;
}