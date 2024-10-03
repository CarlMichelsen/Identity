using System.Text.Json.Serialization;

namespace Domain.OAuth.Github;

public class GithubSimpleUserDto : GithubSimpleUserUrlDto
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("login")]
    public required string Login { get; set; }

    [JsonPropertyName("node_id")]
    public required string NodeId { get; set; }

    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public required string GravatarId { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("site_admin")]
    public required bool SiteAdmin { get; set; }
}