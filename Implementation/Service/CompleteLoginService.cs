using Database.Entity;
using Domain.Abstraction;
using Domain.Dto;
using Domain.OAuth;
using Interface.Factory;
using Interface.Repository;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Service;

public class CompleteLoginService(
    IHttpContextAccessor contextAccessor,
    IUserLoginRepository userLoginRepository,
    IFirstLoginNotifierService firstLoginNotifierService,
    IOAuthClientFactory oAuthClientFactory) : ICompleteLoginService
{
    public async Task<Result<LoginProcessContext>> CompleteLogin(OAuthProvider provider)
    {
        var oAuthClientResult = oAuthClientFactory.Create(provider);
        if (oAuthClientResult.IsError)
        {
            return oAuthClientResult.Error!;
        }

        var qParamResult = this.GetQueryParameters();
        if (qParamResult.IsError)
        {
            return qParamResult.Error!;
        }

        var loginContextResult = await oAuthClientResult
            .Unwrap()
            .GetProviderLoginData(qParamResult.Unwrap());
        if (loginContextResult.IsError)
        {
            return loginContextResult.Error!;
        }

        var loginContext = loginContextResult.Unwrap();
        var userResult = loginContext.OAuthUserConvertible!.ToUser();
        if (userResult.IsError)
        {
            return userResult.Error!;
        }

        var loginUserResult = await userLoginRepository.LoginUser(
            loginContext.OAuthUserConvertible,
            loginContext.Identifier);
        if (loginUserResult.IsError)
        {
            return loginUserResult.Error!;
        }
        
        var loginUser = loginUserResult.Unwrap();
        loginContext.LoginId = loginUser.LoginId;
        loginContext.RefreshId = loginUser.RefreshId;
        loginContext.AccessId = loginUser.AccessId;
        loginContext.User = new AuthenticatedUser(
            Id: loginUser.User.Id,
            Username: loginUser.User.Username,
            Email: loginUser.User.Email,
            AvatarUrl: loginUser.User.AvatarUrl);

        if (loginUser.FirstLogin)
        {
            await firstLoginNotifierService.FirstLogin(loginUser.LoginId);
        }

        return loginContext;
    }

    private Result<Dictionary<string, string>> GetQueryParameters()
    {
        if (contextAccessor.HttpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No HttpContext found");
        }
        
        return contextAccessor.HttpContext.Request.Query
            .Where(kv => kv.Key.Length > 0 && kv.Value.Count > 0)
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Key.First().ToString()))
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value.FirstOrDefault()?.ToString()))
            .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
    }
}