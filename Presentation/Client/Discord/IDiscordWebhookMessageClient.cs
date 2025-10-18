namespace Presentation.Client.Discord;

public interface IDiscordWebhookMessageClient
{
    Task SendMessage(
        WebhookMessage message,
        CancellationToken cancellationToken = default);
}