using Presentation.Client.Discord;

namespace Test.Fakes;

public class FakeDiscordWebhookMessageClient : IDiscordWebhookMessageClient
{
    public IReadOnlyList<WebhookMessage> WebhookMessages { get; } = new List<WebhookMessage>();
    
    public Task SendMessage(WebhookMessage message, CancellationToken cancellationToken = default)
    {
        ((List<WebhookMessage>)WebhookMessages).Add(message);
        return Task.CompletedTask;
    }
}