using System.ComponentModel.DataAnnotations;
using Database;

namespace Presentation.Configuration.Options.Provider;

public abstract class BaseProvider
{
    public abstract AuthenticationProvider ProviderType { get; }
    
    [Required]
    [MinLength(1)]
    public required string ClientId { get; init; }
    
    [Required]
    public required string ClientSecret { get; init; }
    
    [Required]
    [Url]
    public required Uri OAuthEndpoint { get; init; }
}