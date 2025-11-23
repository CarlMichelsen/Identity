namespace Presentation.Configuration.Options.Provider;

public class GitHubProvider : BaseProvider
{
    public override AuthenticationProvider ProviderType { get; } =  AuthenticationProvider.GitHub;
    
    public required Uri ApiUrl { get; init; }
    
    public required Uri UserUrl { get; init; }
}