using Domain.OAuth;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class CompleteLoginHandler(
    IErrorLogService errorLogService,
    ICompleteLoginService completeLoginService,
    ILoginCookieWriterService loginCookieWriterService)
    : ICompleteLoginHandler
{
    public async Task<IResult> CompleteLogin(string authenticationMethod)
    {
        var providerResult = OAuthProviderExtensions.MapToProvider(authenticationMethod);
        if (providerResult.IsError)
        {
            errorLogService.Log(providerResult.Error!);
            return Results.StatusCode(500);
        }
        
        var completeLoginResult = await completeLoginService.CompleteLogin(providerResult.Unwrap());
        if (completeLoginResult.IsError)
        {
            errorLogService.Log(completeLoginResult.Error!);
            return Results.StatusCode(500);
        }
        
        var loginContext = completeLoginResult.Unwrap();
        var cookieResult = loginCookieWriterService.WriteLoginCookies(loginContext);
        if (cookieResult.IsError)
        {
            errorLogService.Log(cookieResult.Error!);
            return Results.StatusCode(500);
        }
        
        return Results.Redirect(loginContext.Redirect.RedirectUri.AbsoluteUri);
    }
}