using System.Text.Json.Serialization;

namespace Domain.User;

public record AuthenticatedUser(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("avatarUrl")] string AvatarUrl);