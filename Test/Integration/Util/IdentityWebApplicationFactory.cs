using Application.Configuration;
using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Presentation.Client.Discord;
using Test.Fakes;

namespace Test.Integration.Util;

public class IdentityWebApplicationFactory : WebApplicationFactory<Program>
{
    public string InMemoryDatabaseName { get; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Optional custom TimeProvider for testing time-dependent behavior.
    /// If set, this will replace the default TimeProvider in the DI container.
    /// </summary>
    public TimeProvider? CustomTimeProvider { get; set; }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure the host BEFORE it's built
        builder.ConfigureHostConfiguration(config =>
        {
            config.Sources.Clear();
            config
                .AddJsonFile("appsettings.testing.json", optional: false)
                .AddEnvironmentVariables();

            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IntegrationTest"] = "true",
            }!);
        });

        return base.CreateHost(builder);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find and remove the original DbContext registration
            var dbContextDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
    
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Also remove the DbContext itself
            var contextDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(DatabaseContext));
    
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            
            // Add in-memory database for testing
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(InMemoryDatabaseName);
            });
            
            // Replace TimeProvider if a custom one is provided
            if (CustomTimeProvider != null)
            {
                var timeProviderDescriptor = services
                    .FirstOrDefault(d => d.ServiceType == typeof(TimeProvider));
                
                if (timeProviderDescriptor != null)
                {
                    services.Remove(timeProviderDescriptor);
                }
                
                services.AddSingleton(CustomTimeProvider);
            }
            
            services.AddSingleton<IHostEnvironment>(new HostingEnvironment
            {
                EnvironmentName = ApplicationConstants.TestEnvironment
            });
            
            // Override other services as needed for testing
            services
                .AddScoped<IDiscordWebhookMessageClient, FakeDiscordWebhookMessageClient>();
        });
        
        builder.UseEnvironment(ApplicationConstants.TestEnvironment);
    }
}