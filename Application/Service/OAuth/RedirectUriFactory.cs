using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration.Options;
using Presentation.Configuration.Options.Provider;
using Presentation.Service.OAuth;

namespace Application.Service.OAuth;

public class RedirectUriFactory(
    IOptionsSnapshot<AuthOptions> authOptions) : IRedirectUriFactory
{
    public Uri CreateRedirectUri(
        AuthenticationProvider authenticationProvider,
        string state)
    {
        var providers = authOptions.Value.GetProviders();
        if (!providers.TryGetValue(authenticationProvider, out var provider))
        {
            throw new OAuthException(
                $"Authentication provider '{nameof(authenticationProvider)}' not implemented");
        }
        
        return authenticationProvider switch
        {
            AuthenticationProvider.Test => this.CreateTestRedirectUri(state, (TestProvider)provider),
            AuthenticationProvider.Discord => this.CreateDiscordRedirectUri(state, (DiscordProvider)provider),
            AuthenticationProvider.GitHub => this.CreateGitHubRedirectUri(state, (GitHubProvider)provider),
            _ => throw new ArgumentOutOfRangeException(nameof(authenticationProvider), authenticationProvider, null)
        };
    }

    private Uri CreateTestRedirectUri(string state, TestProvider provider)
    {
        throw new NotImplementedException(nameof(TestProvider));
    }
    
    private Uri CreateDiscordRedirectUri(string state, DiscordProvider provider)
    {
        throw new NotImplementedException(nameof(DiscordProvider));
    }
    
    private Uri CreateGitHubRedirectUri(string state, GitHubProvider provider)
    {
        throw new NotImplementedException(nameof(GitHubProvider));
    }
}