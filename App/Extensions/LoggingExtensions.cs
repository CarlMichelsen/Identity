using Application.Configuration;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;

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
                .Enrich.WithProperty("Environment", GetEnvironmentName(builder.Environment));
        });

        return builder;
    }
    
    public static WebApplication LogStartup(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            foreach (var address in app.Urls)
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
}