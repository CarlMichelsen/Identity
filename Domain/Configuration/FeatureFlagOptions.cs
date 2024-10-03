namespace Domain.Configuration;

public class FeatureFlagOptions
{
    public const string SectionName = "FeatureFlag";
    
    public required bool DevelopmentLoginEnabled { get; init; }
}