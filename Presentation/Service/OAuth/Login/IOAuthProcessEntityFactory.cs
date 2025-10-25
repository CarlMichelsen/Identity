using Database.Entity;

namespace Presentation.Service.OAuth.Login;

/// <summary>
/// A factory that instantiates <see cref="OAuthProcessEntity" />.
/// </summary>
public interface IOAuthProcessEntityFactory
{
    /// <summary>
    /// Simply creates a <see cref="OAuthProcessEntity" /> instance. Does not save anything to the database.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider.</param>
    /// <param name="successRedirectUrl">Redirect uri if the login succeeds.</param>
    /// <param name="errorRedirectUrl">Redirect uri if the login fails.</param>
    /// <returns></returns>
    OAuthProcessEntity CreateProcess(
        AuthenticationProvider authenticationProvider,
        Uri successRedirectUrl,
        Uri errorRedirectUrl);
}