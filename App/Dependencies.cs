using System.Text.Json.Serialization;
using App.Constraints;
using App.Extensions;
using App.HostedServices;
using App.JsonConverters;
using Application.Client.Discord;
using Application.Configuration.Options;
using Application.Service.OAuth.Login;
using Application.Service.OAuth.Login.Receive;
using AuthProvider.Providers;
using AuthProvider.Providers.Discord;
using AuthProvider.Providers.GitHub;
using AuthProvider.Providers.Test;
using Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation;
using Presentation.Client.Discord;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth.Login;
using Presentation.Service.OAuth.Login.Receive;

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
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add("provider", typeof(EnumConstraint<AuthenticationProvider>));
        });

        builder.Services
            .AddSingleton(TimeProvider.System)
            .AddConfigurationOptions<DiscordWebhookOptions>(builder.Configuration)
            .AddConfigurationOptions<AuthOptions>(builder.Configuration);

        // OpenApi
        builder.Services
            .AddOpenApi();

        // Cors
        builder.Services.AddCors(options =>
        {
            var whitelistedDomainsConfigKey =
                $"{AuthOptions.SectionName}:{nameof(AuthOptions.WhitelistedDomains)}";
            var whitelistedHosts = builder
                .Configuration
                .GetSection(whitelistedDomainsConfigKey)
                .Get<List<Uri>>()?
                .Select(uri => uri.Host) ?? throw new NullReferenceException($"Unable to find {whitelistedDomainsConfigKey} when configuring cors");
            
            options.AddPolicy("whitelist", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin => whitelistedHosts.Contains(new Uri(origin).Host));
            });
        });

        // Logging
        builder.ApplicationUseSerilog();

        // HealthChecks / Middleware
        builder.Services.AddExceptionHandler(_ => {});
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
        
        // Login
        builder.Services
            .AddScoped<ILoginRedirectService, LoginRedirectService>()
            .AddScoped<IOAuthProcessEntityFactory, OAuthProcessEntityFactory>()
            .AddScoped<IRedirectUriFactory, RedirectUriFactory>();
        
        // Login Receiver
        builder.Services
            .AddScoped<DiscordLoginReceiver>()
            .AddScoped<GitHubLoginReceiver>()
            .AddScoped<TestLoginReceiver>()
            .AddScoped<ILoginReceiverFactory, LoginReceiverFactory>();
        
        builder.Services
            .AddScoped<ILoginReceiverRedirectService, LoginReceiverRedirectService>()
            .AddScoped<ILoginEntityFactory, LoginEntityFactory>();
    }
}