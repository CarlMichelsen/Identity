using System.Net.Http.Json;
using Implementation.Configuration.Options;
using Interface.Client.Discord;
using Microsoft.Extensions.Options;

namespace Implementation.Client.Discord;

public class DiscordWebhookMessageClient(
    HttpClient httpClient,
    IOptions<DiscordWebhookOptions> options) : IDiscordWebhookMessageClient
{
    public async Task SendMessageAsync(
        WebhookMessage message,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            options.Value.Url,
            message,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}