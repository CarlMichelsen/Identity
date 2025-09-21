using System.Text;
using System.Threading.Channels;
using Implementation.Configuration;
using Interface.Client.Discord;
using Serilog.Events;

namespace App.HostedServices;

public class LogEscalationProcessor(
    Channel<LogEvent> channel,
    IDiscordWebhookMessageClient discordWebhookMessageClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            try
            {
                var logEvent = await channel.Reader.ReadAsync(stoppingToken);
                await discordWebhookMessageClient.SendMessageAsync(
                    CreateWebhookMessage(logEvent),
                    stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"Discord logging failed - log event will not be sent to discord\n{e.Message}");
            }
        }
    }

    private static WebhookMessage CreateWebhookMessage(LogEvent logEvent)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in logEvent.Properties)
        {
            sb.Append(key);
            sb.Append(' ');
            sb.AppendLine(value.ToString());
        }
        
        var exceptionText = logEvent.Exception is not null
            ? $"*{logEvent.Exception.GetType().Name}*\n{logEvent.Exception.Message}\n\n*{logEvent.Exception.StackTrace ?? "No stack trace"}"
            : null;
        
        const int deepRed = 0x8B0000; // RGB(139, 0, 0)
        return new WebhookMessage(
            Content: logEvent.RenderMessage(),
            Username: ApplicationConstants.Name,
            Embeds: [
                new WebhookEmbed(
                    Title: $"LogEventLevel.{Enum.GetName(logEvent.Level)}",
                    Description: exceptionText,
                    Color: deepRed),
                new WebhookEmbed(
                    Title: "Properties",
                    Description: sb.ToString(),
                    Timestamp: logEvent.Timestamp.DateTime.ToUniversalTime()),
            ]);
    }
}