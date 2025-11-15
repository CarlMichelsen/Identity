using System.Text.Json;
using System.Text.Json.Serialization;
using Presentation;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.Test;

public class TestUser : IAuthenticatedUserConvertible
{
    public required string AuthenticationProviderId { get; init; }
    
    public required string Username { get; init; }
    
    public required string Email { get; init; }
    
    public required Uri AvatarUrl { get; init; }
    
    public AuthenticatedUser GetAuthenticatedUser()
    {
        return new AuthenticatedUser(
            AuthenticationProviderId,
            AuthenticationProvider.Test,
            Username,
            Email,
            AvatarUrl);
    }
    
    [JsonIgnore]
    public string UserJson => JsonSerializer.Serialize(this);
}