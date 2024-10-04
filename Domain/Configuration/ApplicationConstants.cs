namespace Domain.Configuration;

public static class ApplicationConstants
{
    public const string ApplicationName = "Identity";
    
    public const string TraceIdHeaderName = "X-Trace-Id";
    
    public const string DevelopmentCorsPolicyName = "development-cors-policy";
    
    public const string DevelopmentFrontendUrl = "http://localhost:5173";
    
    public const string SecurityAlgorithm = "HS512"; // HmacSha512

    public static TimeSpan AccessTokenLifeTime { get; } = TimeSpan.FromMinutes(5);
    
    public static TimeSpan RefreshTokenLifeTime { get; } = TimeSpan.FromDays(28);
}