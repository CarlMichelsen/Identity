using AuthProvider.Providers.Discord;
using AuthProvider.Providers.GitHub;
using AuthProvider.Providers.Test;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using Presentation.Service.OAuth.LoginCallback;

namespace AuthProvider.Providers;

public class LoginReceiverFactory(
    IServiceScopeFactory serviceScopeFactory) : ILoginReceiverFactory
{
    public ILoginReceiver Create(AuthenticationProvider provider)
    {
        using var scope = serviceScopeFactory.CreateScope();
        return provider switch
        {
            AuthenticationProvider.Test => scope.ServiceProvider.GetRequiredService<TestLoginReceiver>(),
            AuthenticationProvider.Discord => scope.ServiceProvider.GetRequiredService<DiscordLoginReceiver>(),
            AuthenticationProvider.GitHub => scope.ServiceProvider.GetRequiredService<GitHubLoginReceiver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };
    }
}