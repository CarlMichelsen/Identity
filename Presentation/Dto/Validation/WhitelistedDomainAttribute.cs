using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;

namespace Presentation.Dto.Validation;

public class WhitelistedDomainAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not Uri uri)
        {
            return new ValidationResult($"{validationContext.MemberName} is not a {nameof(Uri)}");
        }

        var authOptions = validationContext
            .GetRequiredService<IOptionsSnapshot<AuthOptions>>();

        return authOptions.Value.WhitelistedDomains.Any(whitelistedDomain => IsUriDomainMatch(whitelistedDomain, uri))
            ? ValidationResult.Success
            : new ValidationResult($"{validationContext.MemberName} is not a whitelisted domain");
    }

    private static bool IsUriDomainMatch(Uri whitelistedDomain, Uri uri)
    {
        return string.Equals(whitelistedDomain.Host, uri.Host, StringComparison.OrdinalIgnoreCase);
    }
}