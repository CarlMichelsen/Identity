using System.Text.Json.Serialization;

namespace Interface.Client.Discord;

public record WebhookEmbed(
    [property: JsonPropertyName("title")] string? Title = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("color")] int? Color = null,
    [property: JsonPropertyName("url")] string? Url = null,
    [property: JsonPropertyName("timestamp")] DateTime? Timestamp = null);