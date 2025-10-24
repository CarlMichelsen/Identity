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
        var testOptions = authOptions.Value.Test;
        ArgumentNullException.ThrowIfNull(testOptions);
        var endpointProviderName = Enum.GetName(provider.ProviderType);
        ArgumentNullException.ThrowIfNull(endpointProviderName);
        
        return new OAuthUriBuilder(testOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "code")
            .AddQueryParam("client_id", testOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(endpointProviderName))
            .AddQueryParam("scope", string.Join(' ', testOptions.Scopes))
            .AddQueryParam("state", state)
            .Build();
    }
    
    private Uri CreateDiscordRedirectUri(string state, DiscordProvider provider)
    {
        var discordOptions = authOptions.Value.Discord;
        ArgumentNullException.ThrowIfNull(discordOptions);
        var endpointProviderName = Enum.GetName(provider.ProviderType);
        ArgumentNullException.ThrowIfNull(endpointProviderName);
        
        return new OAuthUriBuilder(discordOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "code")
            .AddQueryParam("client_id", discordOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(endpointProviderName))
            .AddQueryParam("scope", string.Join(' ', discordOptions.Scopes))
            .AddQueryParam("prompt", "consent")
            .AddQueryParam("state", state)
            .Build();
    }
    
    private Uri CreateGitHubRedirectUri(string state, GitHubProvider provider)
    {
        var gitHubOptions = authOptions.Value.GitHub;
        ArgumentNullException.ThrowIfNull(gitHubOptions);
        var endpointProviderName = Enum.GetName(provider.ProviderType);
        ArgumentNullException.ThrowIfNull(endpointProviderName);
        
        return new OAuthUriBuilder(gitHubOptions.OAuthEndpoint)
            .AddQueryParam("response_type", "token")
            .AddQueryParam("client_id", gitHubOptions.ClientId)
            .AddQueryParam("redirect_uri", this.GetRedirectUri(endpointProviderName))
            .AddQueryParam("scope", string.Join(' ', gitHubOptions.Scopes))
            .AddQueryParam("allow_signup", "false")
            .AddQueryParam("state", state)
            .Build();
    }

    private string GetRedirectUri(string providerName)
    {
        var url = new OAuthUriBuilder(authOptions.Value.Self)
            .SetPath($"api/v1/oauth/authorize/{providerName}")
            .ClearQueryParams()
            .Build()
            .AbsoluteUri;

        return Uri.EscapeDataString(url);
    }
}