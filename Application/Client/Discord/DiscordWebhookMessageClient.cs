using System.Net.Http.Json;
using Application.Configuration.Options;
using Microsoft.Extensions.Options;
using Presentation.Client.Discord;

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