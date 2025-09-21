using System.Threading.Channels;
using App.HostedServices;
using Implementation.Client.Discord;
using Implementation.Configuration;
using Interface.Client.Discord;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Events;

namespace App.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ApplicationUseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, sp, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(sp)
                .Enrich.WithOpenTelemetryTraceId()
                .Enrich.WithOpenTelemetrySpanId()
                .Enrich.WithProperty("Application", ApplicationConstants.Name)
                .Enrich.WithProperty("Version", ApplicationConstants.Version)
                .Enrich.WithProperty("Environment", GetEnvironmentName(builder.Environment))
                .Enrich.With(new LogEscalationEnricher(sp));
        });

        builder.Services
            .AddSingleton(
                _ => Channel.CreateUnbounded<LogEvent>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    AllowSynchronousContinuations = false,
                }))
            .AddHostedService<LogEscalationProcessor>()
            .AddHttpClient<IDiscordWebhookMessageClient, DiscordWebhookMessageClient>()
            .AddStandardResilienceHandler();

        return builder;
    }

    public static WebApplication LogStartup(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var addresses = app.Urls;  // For minimal API, we can access urls directly
            foreach (var address in addresses)
            {
                logger.LogInformation(
                    "{ApplicationName} has started in {Mode} mode at {Address}",
                    ApplicationConstants.Name,
                    GetEnvironmentName(app.Environment),
                    address);
            }
        });

        return app;
    }

    private static string GetEnvironmentName(IHostEnvironment environment) =>
        environment.IsProduction() ? "Production" : "Development";

    private class LogEscalationEnricher(IServiceProvider serviceProvider) : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Level < LogEventLevel.Error)
            {
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var channel = scope.ServiceProvider.GetRequiredService<Channel<LogEvent>>();
            channel.Writer.TryWrite(logEvent);
        }
    }
}