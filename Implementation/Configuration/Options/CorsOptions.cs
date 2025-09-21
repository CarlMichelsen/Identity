using Interface.Configuration;

namespace Implementation.Configuration;

public class CorsOptions : IOptionsSection
{
    public static string SectionName => "Cors";

    public required List<string> WhitelistedUrls { get; init; }
}