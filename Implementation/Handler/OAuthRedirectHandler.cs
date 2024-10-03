using Domain.OAuth;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class OAuthRedirectHandler(
    IErrorLogService errorLogService,
    IOAuthRedirectService oAuthRedirectService) : IOAuthRedirectHandler
{
    public async Task<IResult> CreateOAuthRedirect(
        string oAuthProviderString,
        string postLoginRedirectUrl)
    {
        var oauthProviderResult = OAuthProviderExtensions.MapToProvider(oAuthProviderString);
        if (oauthProviderResult.IsError)
        {
            errorLogService.Log(oauthProviderResult.Error!);
            return Results.StatusCode(500);
        }
        
        var redirectInfo = new LoginRedirectInformation
        {
            RedirectUri = new Uri(postLoginRedirectUrl), 
        };

        var redirectUriResult = await oAuthRedirectService
            .CreateOAuthRedirect(oauthProviderResult.Unwrap(), redirectInfo);
        if (redirectUriResult.IsSuccess)
        {
            return Results.Redirect(redirectUriResult.Unwrap().AbsoluteUri);
        }
        
        errorLogService.Log(redirectUriResult.Error!);
        return Results.StatusCode(500);
    }
}