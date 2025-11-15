using System.ComponentModel.DataAnnotations;

namespace Presentation.Configuration.Options;

public class TokenConfiguration : IValidatableObject
{
    private const int MinimumSecretLength = 256;
    
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public required string CookieName { get; init; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public required string JwtIssuer { get; init; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public required string JwtAudience { get; init; }
    
    public required List<string> JwtSecrets { get; init; }
    
    [Required]
    public required TimeSpan Lifetime { get; init; }
    
    /// <summary>
    /// The minimum lifetime that has to be passed before a new token can be generated.
    /// The same token will be returned again if the original token is younger than this timespan.
    /// </summary>
    [Required]
    public required TimeSpan MinimumLifetime { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Lifetime <= MinimumLifetime)
        {
            yield return new ValidationResult("The lifetime must be greater than or equal to MinimumLifetime");
        }

        foreach (var jwtSecret in JwtSecrets)
        {
            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                yield return new ValidationResult("The jwt secret must not be empty");
            }

            if (jwtSecret.Length < MinimumSecretLength)
            {
                yield return new ValidationResult($"All JWT secrets must be at least {MinimumSecretLength} characters");
            }
        }
    }
}