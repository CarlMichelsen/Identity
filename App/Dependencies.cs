using App.Extensions;
using App.HostedServices;
using App.JsonConverters;
using Application.Client.Discord;
using Application.Configuration.Options;
using Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation.Client.Discord;

namespace App;

public static class Dependencies
{
    public static void RegisterIdentityDependencies(
        this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration
            .AddJsonFile(
                "secrets.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
        builder.Services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
        builder.Services
            .AddSingleton(TimeProvider.System)
            .AddConfigurationOptions<DiscordWebhookOptions>(builder.Configuration)
            .AddConfigurationOptions<AuthOptions>(builder.Configuration);

        // OpenApi
        builder.Services
            .AddOpenApi();

        // Logging
        builder.ApplicationUseSerilog();

        // HealthChecks
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        // Queue
        builder.Services
            .AddSingleReaderChannel<WebhookMessage>();

        // HostedServices
        builder.Services
            .AddSingleton<DiscordWebhookQueueProcessor>()
            .AddHostedService<DiscordWebhookQueueProcessor>();
        
        // Database
        builder.AddDatabase<DatabaseContext>();
        
        // Client
        builder.Services
            .AddHttpClient<IDiscordWebhookMessageClient, DiscordWebhookMessageClient>()
            .AddStandardResilienceHandler();
        
        // Services
    }
}