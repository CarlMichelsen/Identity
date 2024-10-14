namespace Domain.Configuration;

public class CorsOptions
{
    public const string SectionName = "Cors";

    public required List<string> WhitelistedUrls { get; init; }
}