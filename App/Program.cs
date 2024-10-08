using App;
using App.Endpoints;
using App.Middleware;
using Domain.Configuration;
using Domain.OAuth;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterApplicationDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // "We are all born ignorant, but one must work hard to remain stupid." - unknown
    await app.Services.EnsureDatabaseUpdated();
}

app.UseMiddleware<EndpointLogMiddleware>();

app.UseMiddleware<UnhandledExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors(ApplicationConstants.DevelopmentCorsPolicyName);
}

var apiGroup = app.MapGroup("api/v1");

apiGroup.RegisterLoginEndpoints();

apiGroup.RegisterUserEndpoints();

var featureFlags = app.Services.GetRequiredService<IOptions<FeatureFlagOptions>>().Value;
if (featureFlags.DevelopmentLoginEnabled)
{
    apiGroup.RegisterDevelopmentEndpoints();
    app.UseStaticFiles(StaticFileOptionsFactory.Create());
    app.MapFallbackToFile("index.html");
}

app.MapGet("health", () => Results.Ok());

app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Identity service has started");

app.Run();