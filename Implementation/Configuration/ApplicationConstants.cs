namespace Implementation.Configuration;

public static class ApplicationConstants
{
    public const string Name = "Identity";

    public const string Version = "0.1.0";
    
    public const string TraceIdHeaderName = "X-Trace-Id";
    public const string IdentityIPHeaderName = "X-Identity-IP";
    
    public const string UnauthorizedErrorMessage = "unauthorized";
    
    public const string DevelopmentCorsPolicyName = "development-cors-policy";

    public const string ProductionCorsPolicyName = "production-cors-policy";
    
    public const string SecurityAlgorithm = "HS512"; // HmacSha512

    public static TimeSpan AccessTokenLifeTime { get; } = TimeSpan.FromMinutes(30);
    
    public static TimeSpan RefreshTokenLifeTime { get; } = TimeSpan.FromDays(28);
    
    public static TimeSpan MinimumRefreshTokenLifeTimeForNewToken { get; } = TimeSpan.FromDays(14);

    public static List<string> DevelopmentCorsUrl { get; } =
    [
        "http://localhost:5773",
        "http://localhost:5666",
        "http://localhost:5667",
        "http://localhost:5444",
        "http://localhost:5445"
    ];
}