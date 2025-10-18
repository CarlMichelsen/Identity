using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class UtcDateTimeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not DateTime dateTime)
        {
            return new ValidationResult("Value is not a date");
        }

        return dateTime.Kind != DateTimeKind.Utc
            ? new ValidationResult("Date is not in UTC format")
            : ValidationResult.Success;
    }
}