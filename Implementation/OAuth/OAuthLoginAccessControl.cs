using Domain.Abstraction;
using Domain.OAuth;
using Interface.OAuth;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.OAuth;

public class OAuthLoginAccessControl(
    IHttpContextAccessor httpContextAccessor,
    ICacheService cacheService) : IOAuthLoginAccessControl
{
    public async Task<Result<LoginProcessContext>> GetProcessIdentifier(
        LoginRedirectInformation loginRedirectInformation)
    {
        var loginProcessResult = LoginContextRetriever
            .GetLoginProcessIdentifier(Guid.NewGuid(), httpContextAccessor.HttpContext);
        if (loginProcessResult.IsError)
        {
            return loginProcessResult.Error!;
        }

        var processIdentifier = loginProcessResult.Unwrap();
        var context = new LoginProcessContext
        {
            Identifier = processIdentifier,
            Redirect = loginRedirectInformation,
        };
        var cacheResult = await cacheService.SetValue(
            context.Identifier.State.ToString(),
            context, 
            TimeSpan.FromMinutes(10));
        if (cacheResult.IsError)
        {
            return cacheResult.Error!;
        }

        return context;
    }
    
    public async Task<Result<LoginProcessContext>> ValidateLoginProcess(
        Dictionary<string, string> queryParameters,
        Func<Dictionary<string, string>,  Result<(Guid State, string Additional)>> stateCode,
        Func<string, Task<Result<IUserConvertible>>> getUser)
    {
        var stateAndCodeResult = stateCode(queryParameters);
        if (stateAndCodeResult.IsError)
        {
            return stateAndCodeResult.Error!;
        }
            
        var sac = stateAndCodeResult.Unwrap();
        var existingLoginContextResult = await cacheService.GetValue<LoginProcessContext>(sac.State.ToString());
        if (existingLoginContextResult.IsError)
        {
            return existingLoginContextResult.Error!;
        }
            
        var requestIdentifierResult = LoginContextRetriever
            .GetLoginProcessIdentifier(sac.State, httpContextAccessor.HttpContext);
        if (requestIdentifierResult.IsError)
        {
            return requestIdentifierResult.Error!;
        }
            
        var existingLoginContext = existingLoginContextResult.Unwrap();
        var requestIdentifier = requestIdentifierResult.Unwrap();
        if (requestIdentifier != existingLoginContext.Identifier)
        {
            return new ResultError(
                ResultErrorType.Unauthorized,
                "Not allowed");
        }

        var userResult = await getUser(sac.Additional);
        if (userResult.IsError)
        {
            return userResult.Error!;
        }

        var removeResult = await cacheService.RemoveValue(sac.State.ToString());
        if (removeResult.IsError)
        {
            return removeResult.Error!;
        }

        existingLoginContext.User = userResult.Unwrap();
        return existingLoginContext;
    }
}