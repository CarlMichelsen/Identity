using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using App.Constraints;
using App.Extensions;
using App.HostedServices;
using App.JsonConverters;
using Application.Client.Discord;
using Application.Configuration;
using Application.Service;
using Application.Service.Image;
using Application.Service.OAuth.Refresh;
using Application.Service.OAuth.Login;
using Application.Service.OAuth.LoginCallback;
using AuthProvider.Providers;
using AuthProvider.Providers.Discord;
using AuthProvider.Providers.GitHub;
using AuthProvider.Providers.Test;
using Database;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Client.Discord;
using Presentation.Configuration.Options;
using Presentation.Service;
using Presentation.Service.Image;
using Presentation.Service.OAuth.Refresh;
using Presentation.Service.OAuth.Login;
using Presentation.Service.OAuth.LoginCallback;

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
            .AddApplicationPart(typeof(Program).Assembly)
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add("provider", typeof(EnumConstraint<AuthenticationProvider>));
        });
        builder.Environment.ApplicationName = ApplicationConstants.Name;

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
        
        // Rate-limiting
        builder.Services.AddRateLimiter(options =>
        {
            // Fixed window rate limiter for images
            options.AddFixedWindowLimiter("images", limiterOptions =>
            {
                limiterOptions.PermitLimit = 3;
                limiterOptions.Window = TimeSpan.FromMinutes(1); // per minute
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10; // Queue up to 10 requests
            });
        });

        // Queue
        builder.Services
            .AddSingleReaderChannel<WebhookMessage>();

        // HostedServices
        builder.Services
            .AddSingleton<DiscordWebhookQueueProcessor>()
            .AddHostedService<DiscordWebhookQueueProcessor>();
        
        builder.Services
            .AddSingleton<ProfileImageProcessor>()
            .AddHostedService<ProfileImageProcessor>();
        
        // Database
        builder.AddDatabase<DatabaseContext>();
        
        // Client
        builder.Services
            .AddHttpContextAccessor()
            .AddHttpClient<IDiscordWebhookMessageClient, DiscordWebhookMessageClient>()
            .AddStandardResilienceHandler();
        
        // Login
        builder.Services
            .AddScoped<ILoginRedirectService, LoginRedirectService>()
            .AddScoped<IOAuthProcessEntityFactory, OAuthProcessEntityFactory>()
            .AddScoped<IRedirectUriFactory, RedirectUriFactory>();
        
        // Login Receiver
        builder.Services.AddHttpClient<DiscordLoginReceiver>((sp, client) =>
        {
            using var scope = sp.CreateScope();
            var authOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AuthOptions>>().Value;
            client.BaseAddress = authOptions.Discord?.ApiUrl;
        });
        builder.Services.AddScoped<GitHubLoginReceiver>();
        builder.Services.AddScoped<TestLoginReceiver>();
        builder.Services.AddScoped<ILoginReceiverFactory, LoginReceiverFactory>();
        
        builder.Services
            .AddScoped<IOAuthRedirectUriFactory, OAuthRedirectUriFactory>()
            .AddScoped<IUserImageUriFactory, UserImageUriFactory>()
            .AddScoped<IUserImageProcessor, UserImageProcessor>()
            .AddScoped<ILogoutService, LogoutService>()
            .AddScoped<IRefreshService, RefreshService>()
            .AddScoped<ITokenRefreshPersistenceService, TokenRefreshPersistenceService>()
            .AddScoped<IFirstLoginNotifier, FirstLoginNotifier>()
            .AddScoped<ITokenValidator, TokenValidator>()
            .AddScoped<ICookieApplier, CookieApplier>()
            .AddScoped<IJsonWebTokenFactory, JsonWebTokenFactory>()
            .AddScoped<ITokenPersistenceService, TokenPersistenceService>()
            .AddScoped<ILoginReceiverRedirectService, LoginReceiverRedirectService>()
            .AddScoped<ILoginEntityFactory, LoginEntityFactory>();
    }
}