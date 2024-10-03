using Domain.OAuth;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class CompleteLoginHandler(
    IErrorLogService errorLogService,
    ICompleteLoginService completeLoginService)
    : ICompleteLoginHandler
{
    public async Task<IResult> CompleteLogin(OAuthProvider provider)
    {
        var completeLoginResult = await completeLoginService.CompleteLogin(provider);
        if (completeLoginResult.IsError)
        {
            errorLogService.Log(completeLoginResult.Error!);
            return Results.StatusCode(500);
        }

        var loginContext = completeLoginResult.Unwrap();
        return Results.Redirect(loginContext.Redirect.RedirectUri.AbsoluteUri);
    }
}