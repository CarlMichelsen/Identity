namespace Presentation.Configuration.Options.Provider;

public class GitHubProvider : BaseProvider
{
    public override AuthenticationProvider ProviderType { get; } =  AuthenticationProvider.GitHub;
}