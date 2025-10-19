namespace AuthProvider.Providers.Test;

public class TestUser : IAuthProviderUserConvertible
{
    public required string AuthenticationProviderId { get; init; }
    
    public required string Username { get; init; }
    
    public required string Email { get; init; }
    
    public required Uri AvatarUrl { get; init; }
    
    public AuthProviderUser GetAuthUser()
    {
        throw new NotImplementedException();
    }
}