namespace Presentation.Service.OAuth.Login;

public interface IRedirectUriFactory
{
    Uri CreateRedirectUri(
        AuthenticationProvider authenticationProvider,
        string state);
}