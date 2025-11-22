namespace Presentation.Service;

public interface IOAuthRedirectUriFactory
{
    Uri GetRedirectUri(AuthenticationProvider provider);
}