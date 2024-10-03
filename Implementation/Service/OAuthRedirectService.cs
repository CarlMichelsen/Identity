using Domain.Abstraction;
using Domain.OAuth;
using Interface.Factory;
using Interface.Service;

namespace Implementation.Service;

public class OAuthRedirectService(
    IOAuthClientFactory oAuthClientFactory) : IOAuthRedirectService
{
    public async Task<Result<Uri>> CreateOAuthRedirect(
        OAuthProvider provider,
        LoginRedirectInformation loginRedirectInformation)
    {
        var oAuthClientResult = oAuthClientFactory.Create(provider);
        if (oAuthClientResult.IsError)
        {
            return oAuthClientResult.Error!;
        }

        var client = oAuthClientResult.Unwrap();
        return await client.CreateOAuthRedirect(loginRedirectInformation);
    }
}