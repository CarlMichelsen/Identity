using System.Text.Json.Serialization;

namespace Domain.Dto;

public record UserDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("avatarUrl")] string AvatarUrl);