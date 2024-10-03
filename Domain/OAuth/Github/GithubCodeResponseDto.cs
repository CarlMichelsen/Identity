using System.Text.Json.Serialization;

namespace Domain.OAuth.Github;

public class GithubCodeResponseDto
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public required string Scope { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
}