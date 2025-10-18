using System.ComponentModel.DataAnnotations;

namespace Application.Configuration.Options.Provider;

public abstract class BaseProvider
{
    [Required]
    [MinLength(1)]
    public required string ClientId { get; init; }
    
    [Required]
    public required string ClientSecret { get; init; }
    
    [Required]
    [Url]
    public required Uri OAuthEndpoint { get; init; }
    
    [Required]
    [Url]
    public required Uri OAuthReturnEndpoint { get; init; }
}