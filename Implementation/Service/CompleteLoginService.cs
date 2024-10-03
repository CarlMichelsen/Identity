using Domain.Abstraction;
using Domain.OAuth;
using Interface.Factory;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Service;

public class CompleteLoginService(
    IHttpContextAccessor contextAccessor,
    IOAuthClientFactory oAuthClientFactory) : ICompleteLoginService
{
    public async Task<Result<LoginProcessContext>> CompleteLogin(OAuthProvider provider)
    {
        var loginContextResult = await this.CompleteLoginAndGetContext(provider);
        if (loginContextResult.IsError)
        {
            return loginContextResult.Error!;
        }

        var userResult = loginContextResult.Unwrap().User!.ToUser();
        if (userResult.IsError)
        {
            return userResult.Error!;
        }

        throw new Exception(userResult.Unwrap().Email);
    }

    private async Task<Result<LoginProcessContext>> CompleteLoginAndGetContext(OAuthProvider provider)
    {
        var oAuthClientResult = oAuthClientFactory.Create(provider);
        if (oAuthClientResult.IsError)
        {
            return oAuthClientResult.Error!;
        }

        if (contextAccessor.HttpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No HttpContext found");
        }
        
        var queryParameters = contextAccessor.HttpContext.Request.Query
            .Where(kv => kv.Key.Length > 0 && kv.Value.Count > 0)
            .ToDictionary(kv => kv.Key.First().ToString(), kv => kv.Value.First()!.ToString());

        return await oAuthClientResult
            .Unwrap()
            .CompleteLogin(queryParameters);
    }
}