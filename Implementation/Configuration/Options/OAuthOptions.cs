using Interface.Configuration;

namespace Implementation.Configuration;

public class OAuthOptions : IOptionsSection
{
    public static string SectionName => "OAuth";
    
    public required string AllowedRedirectDomain { get; init; } 
    
    public required OAuthProviderSection Discord { get; init; }

    public required OAuthProviderSection Github { get; init; }

    public OAuthProviderSection? Development { get; init; }
}