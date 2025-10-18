using Presentation.Dto.Validation;
using Shouldly;

namespace Test.Validation.Attribute;

public class UtcDateTimeAttributeTests
{
    [Fact]
    public void NonUtcKindDateTimeShouldNotBeValid()
    {
        // Arrange
        var validationAttribute = new UtcDateTimeAttribute();

        // Act
        var unspecifiedKindDateTime =
            new DateTime(2025, 10, 18, 12, 0, 0);
        var validationResult = validationAttribute.IsValid(unspecifiedKindDateTime);

        // Assert
        validationResult.ShouldBeFalse();
    }
    
    [Fact]
    public void NUtcKindDateTimeShouldBeValid()
    {
        // Arrange
        var validationAttribute = new UtcDateTimeAttribute();

        // Act
        var validationResult = validationAttribute.IsValid(DateTime.UtcNow);

        // Assert
        validationResult.ShouldBeTrue();
    }
}