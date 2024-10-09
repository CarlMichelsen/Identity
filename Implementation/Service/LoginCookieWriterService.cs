using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Implementation.Map;
using Implementation.Util;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class LoginCookieWriterService(
    IOptions<FeatureFlagOptions> featureFlags,
    IOptions<JwtOptions> jwtOptions,
    IOptions<IdentityCookieOptions> cookieOptions,
    IHttpContextAccessor contextAccessor) : ILoginCookieWriterService
{
    public Result WriteLoginCookies(
        LoginProcessContext processContext)
    {
        if (contextAccessor.HttpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No HttpContext found");
        }

        if (processContext.LoginId is null)
        {
            return new ResultError(ResultErrorType.MapError, "Login id is null");
        }
        
        if (processContext.RefreshId is null)
        {
            return new ResultError(ResultErrorType.MapError, "Refresh id is null");
        }
        
        if (processContext.AccessId is null)
        {
            return new ResultError(ResultErrorType.MapError, "Access id is null");
        }
        
        if (processContext.User is null)
        {
            return new ResultError(ResultErrorType.MapError, "User is null");
        }

        var tokenPairResult = JwtMapper.GetTokenPair(
            loginId: (long)processContext.LoginId,
            refreshId: (long)processContext.RefreshId,
            accessId: (long)processContext.AccessId,
            authenticatedUser: processContext.User,
            jwtOptions: jwtOptions.Value);
        if (tokenPairResult.IsError)
        {
            return tokenPairResult.Error!;
        }

        try
        {
            var tokenPair = tokenPairResult.Unwrap();
            var cookieConfigurationOptions = CookieOptionsFactory.CreateOptions(featureFlags.Value);
        
            contextAccessor.HttpContext.Response.Cookies.Append(
                cookieOptions.Value.AccessCookieName,
                tokenPair.AccessToken,
                cookieConfigurationOptions);
            
            contextAccessor.HttpContext.Response.Cookies.Append(
                cookieOptions.Value.RefreshCookieName,
                tokenPair.RefreshToken,
                cookieConfigurationOptions);
            
            return new Result();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "An exception was thrown while writing cookie pair",
                e);
        }
    }
}