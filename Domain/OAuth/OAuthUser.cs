namespace Domain.OAuth;

public class OAuthUser
{
    public required string Id { get; init; }
    
    public required string AuthenticationProvider { get; init; }
    
    public required string Username { get; init; }
    
    public required string AvatarUrl { get; init; }
    
    public required string Email { get; init; }
}