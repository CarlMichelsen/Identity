using Domain.Abstraction;
using Domain.OAuth;
using Interface.Factory;
using Interface.Service;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class OAuthRedirectService(
    IRedirectValidationService redirectValidationService,
    IOAuthClientFactory oAuthClientFactory) : IOAuthRedirectService
{
    public async Task<Result<Uri>> CreateOAuthRedirect(
        OAuthProvider provider,
        LoginRedirectInformation loginRedirectInformation)
    {
        var validationResult = await redirectValidationService.ValidateRedirect(loginRedirectInformation, provider);
        if (validationResult.IsError)
        {
            return validationResult.Error!;
        }
        
        var oAuthClientResult = oAuthClientFactory.Create(provider);
        if (oAuthClientResult.IsError)
        {
            return oAuthClientResult.Error!;
        }

        var client = oAuthClientResult.Unwrap();
        return await client.CreateOAuthRedirect(loginRedirectInformation);
    }
}