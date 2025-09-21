using Interface.Configuration;

namespace Implementation.Configuration;

public class IdentityCookieOptions : IOptionsSection
{
    public static string SectionName => "Cookie";
    
    public required string AccessCookieName { get; init; }
    
    public required string RefreshCookieName { get; init; }
}