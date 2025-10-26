using App.Controllers;
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
    public string InMemoryDatabaseName { get; } = Guid.NewGuid().ToString();

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
            services.RemoveAllEfCoreServices();
            
            // Add in-memory database for testing
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(InMemoryDatabaseName);
            });
            
            // There are assembly issues when registering controllers, so I have to do this :'(
            services.AddControllers()
                .AddApplicationPart(typeof(LoginController).Assembly);
            
            // Override other services as needed for testing
            services
                .AddScoped<IDiscordWebhookMessageClient, FakeDiscordWebhookMessageClient>();
        });
        
        builder.UseEnvironment("testing");
    }
}