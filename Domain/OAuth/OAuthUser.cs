using Database.Entity;

namespace Domain.OAuth;

public class OAuthUser
{
    public required string ProviderId { get; init; }
    
    public required OAuthProvider AuthenticationProvider { get; init; }
    
    public required string Username { get; init; }
    
    public required string AvatarUrl { get; init; }
    
    public required string Email { get; init; }
}