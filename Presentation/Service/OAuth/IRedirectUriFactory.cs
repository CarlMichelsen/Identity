namespace Presentation.Service.OAuth;

public interface IRedirectUriFactory
{
    Uri CreateRedirectUri(
        AuthenticationProvider authenticationProvider,
        string state);
}