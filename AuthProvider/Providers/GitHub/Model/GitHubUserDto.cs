using System.Text.Json;
using System.Text.Json.Serialization;
using Presentation;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.GitHub.Model;

public class GitHubUserDto : GitHubSimpleUserDto, IAuthenticatedUserConvertible
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    public AuthenticatedUser GetAuthenticatedUser()
    {
        return new AuthenticatedUser(
            AuthenticationProviderId: this.Id.ToString(),
            AuthenticationProvider: AuthenticationProvider.GitHub,
            Username: this.Login,
            Email: this.Email,
            AvatarUrl: new Uri(this.AvatarUrl));
    }

    [JsonIgnore] public string UserJson => JsonSerializer.Serialize(this);
}