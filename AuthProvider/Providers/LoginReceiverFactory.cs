using AuthProvider.Providers.Discord;
using AuthProvider.Providers.GitHub;
using AuthProvider.Providers.Test;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using Presentation.Service.OAuth.Login.Receive;

namespace AuthProvider.Providers;

public class LoginReceiverFactory(
    IServiceProvider serviceProvider) : ILoginReceiverFactory
{
    public ILoginReceiver Create(AuthenticationProvider provider)
    {
        return provider switch
        {
            AuthenticationProvider.Test => serviceProvider.GetRequiredService<TestLoginReceiver>(),
            AuthenticationProvider.Discord => serviceProvider.GetRequiredService<DiscordLoginReceiver>(),
            AuthenticationProvider.GitHub => serviceProvider.GetRequiredService<GitHubLoginReceiver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };
    }
}