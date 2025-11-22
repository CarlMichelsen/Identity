using System.Threading.Channels;
using Presentation.Client.Discord;

namespace App.HostedServices;

public class DiscordWebhookQueueProcessor(
    Channel<WebhookMessage> channel,
    ILogger<DiscordWebhookQueueProcessor> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            try
            {
                var webhookMessage = await channel.Reader.ReadAsync(stoppingToken);

                await using var scope = serviceProvider.CreateAsyncScope();
                var discordWebhookMessageClient = scope.ServiceProvider
                    .GetRequiredService<IDiscordWebhookMessageClient>();
                
                await discordWebhookMessageClient.SendMessage(
                    webhookMessage,
                    stoppingToken);
                
                // Wait 5 seconds between each discord api call.
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Discord message processing was cancelled");
            }
            catch (Exception e)
            {
                var waitTime = TimeSpan.FromMinutes(2);
                logger.LogError(
                    e,
                    "Discord message processing failed - message will not be sent to discord and further message processing will be delayed by {TimeSpan}",
                    waitTime);
                await Task.Delay(waitTime, stoppingToken);
            }
        }
    }
}
