namespace Domain.Configuration;

public class IdentityCookieOptions
{
    public const string SectionName = "Cookie";
    
    public required string AccessCookieName { get; init; }
    
    public required string RefreshCookieName { get; init; }
}