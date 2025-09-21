using Interface.Configuration;

namespace Implementation.Configuration;

public class JwtOptions : IOptionsSection
{
    public static string SectionName => "Jwt";
    
    public required string AccessSecret { get; init; }
    
    public required string RefreshSecret { get; init; }
    
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
}