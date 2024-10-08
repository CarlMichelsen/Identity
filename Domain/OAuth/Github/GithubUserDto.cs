using System.Text.Json.Serialization;
using Database.Entity;
using Domain.Abstraction;

namespace Domain.OAuth.Github;

public class GithubUserDto : GithubSimpleUserDto, IOAuthUserConvertible
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    public Result<OAuthUser> ToUser()
    {
        return OAuthValidator.Validate(
            id: this.Id.ToString(),
            provider: OAuthProvider.Github,
            username: this.Login,
            email: this.Email,
            avatarUrl: this.AvatarUrl);
    }
}