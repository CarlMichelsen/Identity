using System.Text.Json.Serialization;

namespace Domain.OAuth.Development;

public class DevelopmentCodeDto
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
}