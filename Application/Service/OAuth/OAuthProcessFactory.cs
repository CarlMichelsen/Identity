using Database.Entity;
using Presentation;
using Presentation.Service.OAuth;

namespace Application.Service.OAuth;

public class OAuthProcessFactory(
    IRedirectUriFactory redirectUriFactory) : IOAuthProcessFactory
{
    public OAuthProcessEntity CreateProcess(AuthenticationProvider authenticationProvider)
    {
        var state = CreateState();
        var uri = redirectUriFactory.CreateRedirectUri(authenticationProvider, state);
        
        
        
        throw new NotImplementedException(uri.AbsoluteUri);
    }

    private static string CreateState() => Guid.NewGuid().ToString();
}