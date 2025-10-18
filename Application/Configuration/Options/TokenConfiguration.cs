using System.ComponentModel.DataAnnotations;

namespace Application.Configuration.Options;

public class TokenConfiguration
{
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public required string CookieName { get; init; }
    
    [Required]
    public required TimeSpan Lifetime { get; init; }
}