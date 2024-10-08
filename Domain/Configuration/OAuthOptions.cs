namespace Domain.Configuration;

public class OAuthOptions
{
    public const string SectionName = "OAuth";
    
    public required string AllowedRedirectDomain { get; init; } 
    
    public required OAuthProviderSection Discord { get; init; }

    public required OAuthProviderSection Github { get; init; }

    public OAuthProviderSection? Development { get; init; }
}