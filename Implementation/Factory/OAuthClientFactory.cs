using Database.Entity;
using Domain.Abstraction;
using Implementation.Configuration;
using Implementation.Service.OAuth.Client;
using Interface.Factory;
using Interface.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Implementation.Factory;

public class OAuthClientFactory(
    IOptions<OAuthOptions> oAuthOptions,
    IServiceProvider serviceProvider) : IOAuthClientFactory
{
    public Result<IOAuthClient> Create(OAuthProvider provider)
        => provider switch
        {
            OAuthProvider.Development => oAuthOptions.Value.Development is not null
                ? this.GetOAuthLoginClient<DevelopmentLoginClient>()
                : UnsupportedOAuthClient(provider),
            OAuthProvider.Discord => this.GetOAuthLoginClient<DiscordLoginClient>(),
            OAuthProvider.Github => this.GetOAuthLoginClient<GithubLoginClient>(),
            _ => UnsupportedOAuthClient(provider),
        };
    
    private static ResultError UnsupportedOAuthClient(OAuthProvider provider)
    {
        var providerName = Enum.GetName(provider);
        var desc = string.IsNullOrWhiteSpace(providerName)
            ? $"OAuthProvider \"{(int)provider}\" is unknown and not supported"
            : $"OAuthProvider {providerName} is not supported";
        
        return new ResultError(
            ResultErrorType.MapError,
            desc);
    }
    
    private Result<IOAuthClient> GetOAuthLoginClient<T>()
        where T : IOAuthClient
    {
        try
        {
            return serviceProvider.GetRequiredService<T>();
        }
        catch (InvalidOperationException e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                $"Unable to instantiate {nameof(T)} object",
                e);
        }
    }
}