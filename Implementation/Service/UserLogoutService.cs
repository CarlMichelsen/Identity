using Domain.Abstraction;
using Domain.Configuration;
using Domain.Dto;
using Implementation.Util;
using Interface.Repository;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class UserLogoutService(
    IOptions<FeatureFlagOptions> featureFlags,
    IOptions<IdentityCookieOptions> identityOptions,
    IHttpContextAccessor httpContextAccessor,
    IUserLogoutRepository userLogoutRepository) : IUserLogoutService
{
    public async Task<Result<ServiceResponse>> Logout()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No httpContext found");
        }
        
        var loginIdClaim = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == "login")?.Value;
        if (!long.TryParse(loginIdClaim, out var loginId))
        {
            return new ResultError(
                ResultErrorType.NotFound,
                "LoginId not found");
        }

        var logoutResult = await userLogoutRepository.Logout(loginId);
        if (logoutResult.IsError)
        {
            return logoutResult.Error!;
        }
        
        var deleteCookieOptions = CookieOptionsFactory.CreateOptions(featureFlags.Value);
        httpContextAccessor.HttpContext.Response.Cookies.Delete(
            identityOptions.Value.AccessCookieName,
            deleteCookieOptions);
        httpContextAccessor.HttpContext.Response.Cookies.Delete(
            identityOptions.Value.RefreshCookieName,
            deleteCookieOptions);
        
        return new ServiceResponse();
    }
}