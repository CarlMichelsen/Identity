using System.Text.Json.Serialization;

namespace Domain.OAuth.Github;

public class GithubSimpleUserUrlDto
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public required string FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public required string FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public required string GistsUrl { get; set; }

    [JsonPropertyName("starred_url")]
    public required string StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public required string SubscriptionsUrl { get; set; }

    [JsonPropertyName("organizations_url")]
    public required string OrganizationsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public required string ReposUrl { get; set; }

    [JsonPropertyName("events_url")]
    public required string EventsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public required string ReceivedEventsUrl { get; set; }
}