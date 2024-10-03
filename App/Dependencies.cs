using App.Middleware;
using Database;
using Domain.Configuration;
using Implementation.Factory;
using Implementation.Handler;
using Implementation.OAuth;
using Implementation.OAuth.Client;
using Implementation.Repository;
using Implementation.Service;
using Interface.Factory;
using Interface.Handler;
using Interface.OAuth;
using Interface.Repository;
using Interface.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace App;

public static class Dependencies
{
    public static void RegisterApplicationDependencies(
        this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration.AddJsonFile(
            "secrets.json",
            optional: builder.Environment.IsDevelopment(),
            reloadOnChange: true);
        
        builder.Services
            .Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName))
            .Configure<OAuthOptions>(builder.Configuration.GetSection(OAuthOptions.SectionName))
            .Configure<FeatureFlagOptions>(builder.Configuration.GetSection(FeatureFlagOptions.SectionName));
        
        // Configure Serilog from "appsettings.(env).json
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Application", ApplicationConstants.ApplicationName)
            .Enrich.WithProperty("Environment", GetEnvironmentName(builder))
            .CreateLogger();
        builder.Host.UseSerilog();
        
        // Database
        builder.Services.AddDbContext<ApplicationContext>(
            options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    (b) => b.MigrationsAssembly("App"));
                
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });
        
        // Cache
        builder.Services
            .AddMemoryCache();
        
        // Http
        builder.RegisterHttpClients();
        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddScoped<EndpointLogMiddleware>()
            .AddScoped<UnhandledExceptionMiddleware>();
        
        if (builder.Environment.IsDevelopment())
        {
            // Development CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    ApplicationConstants.DevelopmentCorsPolicyName,
                    configurePolicy =>
                    {
                        configurePolicy
                            .WithOrigins(ApplicationConstants.DevelopmentFrontendUrl)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
        }
        
        // Repositories
        builder.Services
            .AddScoped<IUserRepository, UserRepository>();
        
        // Services
        builder.Services
            .AddScoped<IErrorLogService, ErrorLogService>()
            .AddScoped<IOAuthRedirectService, OAuthRedirectService>()
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<ICompleteLoginService, CompleteLoginService>()
            .AddScoped<IDevelopmentUserService, DevelopmentUserService>()
            .AddScoped<ICompleteLoginService, CompleteLoginService>();
        
        // Handlers
        builder.Services
            .AddScoped<IOAuthRedirectHandler, OAuthRedirectHandler>()
            .AddScoped<ICompleteLoginHandler, CompleteLoginHandler>();
        
        // OAuth
        builder.Services
            .AddScoped<IOAuthLoginAccessControl, OAuthLoginAccessControl>()
            .AddScoped<IOAuthClientFactory, OAuthClientFactory>();
        
        // OAuthClients
        builder.Services
            .AddTransient<DevelopmentLoginClient>()
            .AddTransient<DiscordLoginClient>()
            .AddTransient<GithubLoginClient>();
        
        // Auth dependencies
        builder.RegisterAuthDependencies();
    }
    
    private static void RegisterHttpClients(this WebApplicationBuilder builder)
    {
        var oAuthOptions = builder.Configuration
            .GetSection(OAuthOptions.SectionName)
            .Get<OAuthOptions>();

        builder.Services.AddHttpClient<DiscordLoginClient>(options =>
        {
            var uri = new Uri(oAuthOptions!.Discord.OAuthEndpoint);
            options.BaseAddress = new Uri($"{uri.Scheme}://{uri.Host}");
            options.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient<GithubLoginClient>(options =>
        {
            var uri = new Uri(oAuthOptions!.Github.OAuthEndpoint);
            options.BaseAddress = new Uri($"{uri.Scheme}://{uri.Host}");
            options.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }

    private static void RegisterAuthDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorizationBuilder();
    }

    private static string GetEnvironmentName(WebApplicationBuilder builder) =>
        builder.Environment.IsProduction() ? "Production" : "Development";
}