namespace Presentation;

public static class JwtTokenKeys
{
    private const string ClaimTypeNamespace = "http://schemas.microsoft.com/ws/2008/06/identity/claims";
    
    public const string Sub = "sub";
    
    public const string Name = "name";
    
    public const string Email = "email";
    
    public const string Jti = "jti";
    
    public const string Iat = "iat";
    
    public const string Role = $"{ClaimTypeNamespace}/role";
    
    public const string Provider = "provider";
    
    public const string ProviderId = "provider-id";
    
    public const string Profile = "profile";
    
    public const string ProfileMedium = "profile-medium";
    
    public const string ProfileLarge = "profile-large";
    
    public const string Issuer = "iss";
    
    public const string Audience = "aud";
}