using System.ComponentModel.DataAnnotations;
using Application.Configuration.Options;
using Presentation.Configuration.Options.Provider;

namespace Presentation.Configuration.Options;

public class AuthOptions : IConfigurationOptions
{
    public static string SectionName => "Auth";
    
    /// <summary>
    /// This is the url of the identity solution - this is used to generate links.
    /// </summary>
    [Required]
    public required Uri Self { get; init; }
    
    /// <summary>
    /// These are the domains that the user can be redirected back to.
    /// The cors configuration of this application also allow requests to come in from these domains.
    /// </summary>
    public required List<Uri> WhitelistedDomains { get; init; }
    
    /// <summary>
    /// Configuration for the short-lived 'access_token'.
    /// </summary>
    [Required]
    public required TokenConfiguration AccessToken { get; init; }
    
    /// <summary>
    /// Configuration for the longer lifetime 'refresh_token'.
    /// </summary>
    [Required]
    public required TokenConfiguration RefreshToken { get; init; }

    public TestProvider? Test { get; init; }
    
    public DiscordProvider? Discord { get; init; }
    
    public GitHubProvider? GitHub { get; init; }

    public Dictionary<AuthenticationProvider, BaseProvider> GetProviders()
    {
        var dict =  new Dictionary<AuthenticationProvider, BaseProvider>();
        if (this.Test is not null)
        {
            dict.Add(AuthenticationProvider.Test, Test);
        }
        
        if (this.Discord is not null)
        {
            dict.Add(AuthenticationProvider.Discord, Discord);
        }
        
        if (this.GitHub is not null)
        {
            dict.Add(AuthenticationProvider.GitHub, GitHub);
        }

        return dict;
    }
}