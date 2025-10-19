using Domain.User;

namespace Presentation.Configuration.Options.Provider;

public class GithubProvider : BaseProvider
{
    public override AuthenticationProvider ProviderType { get; } =  AuthenticationProvider.Github;
}