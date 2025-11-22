using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration.Options;
using Presentation.Configuration.Options.Provider;
using Presentation.Service;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Login;

namespace Application.Service.OAuth.Login;

public class RedirectUriFactory(
    IOAuthRedirectUriFactory  oAuthRedirectUriFactory,
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
            AuthenticationProvider.Test => this.CreateTestRedirectUri(state, (provider as TestProvider)!),
            AuthenticationProvider.Discord => this.CreateDiscordRedirectUri(state, (provider as DiscordProvider)!),
            AuthenticationProvider.GitHub => this.CreateGitHubRedirectUri(state, (provider as GitHubProvider)!),
            _ => throw new ArgumentOutOfRangeException(nameof(authenticationProvider), authenticationProvider, null)
        };
    }

    private Uri CreateTestRedirectUri(string state, TestProvider provider)
    {
        var testOptions = authOptions.Value.Test;
        ArgumentNullException.ThrowIfNull(testOptions);
        
        return new OAuthUriBuilder(testOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "code")
            .AddQueryParam("client_id", testOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(provider.ProviderType))
            .AddQueryParam("scope", string.Join(' ', testOptions.Scopes))
            .AddQueryParam("state", state)
            .Build();
    }
    
    private Uri CreateDiscordRedirectUri(string state, DiscordProvider provider)
    {
        var discordOptions = authOptions.Value.Discord;
        ArgumentNullException.ThrowIfNull(discordOptions);
        
        return new OAuthUriBuilder(discordOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "code")
            .AddQueryParam("client_id", discordOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(provider.ProviderType))
            .AddQueryParam("scope", string.Join(' ', discordOptions.Scopes))
            .AddQueryParam("prompt", "consent")
            .AddQueryParam("state", state)
            .Build();
    }
    
    private Uri CreateGitHubRedirectUri(string state, GitHubProvider provider)
    {
        var gitHubOptions = authOptions.Value.GitHub;
        ArgumentNullException.ThrowIfNull(gitHubOptions);
        
        return new OAuthUriBuilder(gitHubOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "token")
            .AddQueryParam("client_id", gitHubOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(provider.ProviderType))
            .AddQueryParam("scope", string.Join(' ', gitHubOptions.Scopes))
            .AddQueryParam("allow_signup", "false")
            .AddQueryParam("state", state)
            .Build();
    }

    private string GetRedirectUri(AuthenticationProvider provider)
    {
        var url = oAuthRedirectUriFactory.GetRedirectUri(provider)
            .AbsoluteUri;

        return Uri.EscapeDataString(url);
    }
}