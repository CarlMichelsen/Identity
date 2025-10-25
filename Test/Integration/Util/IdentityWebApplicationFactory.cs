using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Client.Discord;
using Test.Fakes;

namespace Test.Integration.Util;

public class IdentityWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure the host BEFORE it's built
        builder.ConfigureHostConfiguration(config =>
        {
            config.Sources.Clear();
            config
                .AddJsonFile("appsettings.testing.json", optional: false)
                .AddEnvironmentVariables();
        });

        return base.CreateHost(builder);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // Add in-memory database for testing
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
            
            // Override other services as needed for testing
            services
                .AddScoped<IDiscordWebhookMessageClient, FakeDiscordWebhookMessageClient>();
        });
        
        builder.UseEnvironment("testing");
    }
}