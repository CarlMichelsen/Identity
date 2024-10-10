using System.Text.Json;
using Domain.Abstraction;
using Domain.User;
using Implementation.Map;
using Interface.Accessor;
using Microsoft.AspNetCore.Http;

namespace Implementation.Accessor;

public class UserContextAccessor(
    IHttpContextAccessor httpContextAccessor) : IUserContextAccessor
{
    public Result<UserContext> GetUserContext()
    {
        try
        {
            if (httpContextAccessor.HttpContext is null)
            {
                return new ResultError(
                    ResultErrorType.MapError,
                    "No http-context found when getting user");
            }
            
            if (httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                return new ResultError(
                    ResultErrorType.Unauthorized,
                    "Not authorized when accessing user");
            }

            var userJsonClaimResult = GetClaim(httpContextAccessor.HttpContext, JwtMapper.UserClaim);
            if (userJsonClaimResult.IsError)
            {
                return userJsonClaimResult.Error!;
            }

            var user = JsonSerializer.Deserialize<AuthenticatedUser>(userJsonClaimResult.Unwrap());
            if (user is null)
            {
                return new ResultError(
                    ResultErrorType.MapError,
                    "User json resulted in a null object");
            }
            
            var accessIdClaimResult = GetClaim(httpContextAccessor.HttpContext, JwtMapper.AccessIdClaim);
            if (accessIdClaimResult.IsError)
            {
                return accessIdClaimResult.Error!;
            }

            if (!long.TryParse(accessIdClaimResult.Unwrap(), out var accessId))
            {
                return new ResultError(ResultErrorType.MapError, "Failed to map claim to long (accessId)");
            }
            
            var refreshIdClaimResult = GetClaim(httpContextAccessor.HttpContext, JwtMapper.RefreshIdClaim);
            if (refreshIdClaimResult.IsError)
            {
                return refreshIdClaimResult.Error!;
            }
            
            if (!long.TryParse(refreshIdClaimResult.Unwrap(), out var refreshId))
            {
                return new ResultError(ResultErrorType.MapError, "Failed to map claim to long (refreshId)");
            }
            
            var loginIdClaimResult = GetClaim(httpContextAccessor.HttpContext, JwtMapper.LoginIdClaim);
            if (loginIdClaimResult.IsError)
            {
                return loginIdClaimResult.Error!;
            }
            
            if (!long.TryParse(loginIdClaimResult.Unwrap(), out var loginId))
            {
                return new ResultError(ResultErrorType.MapError, "Failed to map claim to long (loginId)");
            }

            return new UserContext(
                LoginId: loginId,
                RefreshId: refreshId,
                AccessId: accessId,
                User: user);
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Getting user from http-context resulted in an exception",
                e);
        }
    }

    private static Result<string> GetClaim(HttpContext httpContext, string claimType)
    {
        var claim = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value;
        if (string.IsNullOrWhiteSpace(claim))
        {
            return new ResultError(
                ResultErrorType.NotFound,
                $"Did not find \"{claimType}\" claim type");
        }

        return claim;
    }
}