using Application.Service.OAuth;
using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration.Options;
using Presentation.Service;

namespace Application.Service;

public class OAuthRedirectUriFactory(
    IOptionsSnapshot<AuthOptions> authOptions) : IOAuthRedirectUriFactory
{
    public Uri GetRedirectUri(AuthenticationProvider provider)
    {
        var providerName = Enum.GetName(provider);
        return new OAuthUriBuilder(authOptions.Value.Self)
            .ClearQueryParams()
            .SetPath($"api/v1/oauth/authorize/{providerName}")
            .Build();
    }
}