using Database.Entity;
using Database.Entity.Id;
using Presentation;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Login;

namespace Application.Service.OAuth.Login;

/// <inheritdoc />
public class OAuthProcessEntityFactory(
    TimeProvider timeProvider,
    IRedirectUriFactory redirectUriFactory) : IOAuthProcessEntityFactory
{
    public OAuthProcessEntity CreateProcess(
        AuthenticationProvider authenticationProvider,
        Uri successRedirectUrl,
        Uri errorRedirectUrl)
    {
        var state = CreateState();
        var uri = redirectUriFactory.CreateRedirectUri(authenticationProvider, state);

        var authenticationProviderString = Enum.GetName(authenticationProvider)?.ToUpperInvariant()
                                           ?? throw new OAuthException($"Unable to convert {nameof(AuthenticationProvider)} '{authenticationProvider}' to a valid non-null string for the database.");

        return new OAuthProcessEntity
        {
            Id = new OAuthProcessEntityId(Guid.CreateVersion7()),
            AuthenticationProvider = authenticationProviderString,
            State =  state,
            LoginRedirectUri = uri,
            SuccessRedirectUri =  successRedirectUrl,
            ErrorRedirectUri =  errorRedirectUrl,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
    }

    private static string CreateState() => Guid.NewGuid().ToString();
}