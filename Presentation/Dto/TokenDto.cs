using System.Text.Json.Serialization;

namespace Presentation.Dto;

public record TokenDto(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("refresh_token_expires_in")] int RefreshTokenExpiresIn,
    [property: JsonPropertyName("scope")] string Scope);