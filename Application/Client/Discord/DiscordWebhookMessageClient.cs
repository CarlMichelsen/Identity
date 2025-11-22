using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Presentation.Client.Discord;
using Presentation.Configuration.Options;

namespace Application.Client.Discord;

public class DiscordWebhookMessageClient(
    HttpClient httpClient,
    IOptionsSnapshot<DiscordWebhookOptions> options) : IDiscordWebhookMessageClient
{
    public async Task SendMessage(
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