using System.Text.Json.Serialization;

namespace Interface.Client.Discord;

public record WebhookMessage(
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("username")] string? Username = null,
    [property: JsonPropertyName("avatar_url")] string? AvatarUrl = null,
    [property: JsonPropertyName("embeds")] WebhookEmbed[]? Embeds = null);