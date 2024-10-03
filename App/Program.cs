using App;
using App.Endpoints;
using App.Middleware;
using Domain.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterApplicationDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<EndpointLogMiddleware>();

app.UseMiddleware<UnhandledExceptionMiddleware>();

var apiGroup = app.MapGroup("api/v1");

apiGroup.RegisterLoginEndpoints(
    app.Services.GetRequiredService<IOptions<FeatureFlagOptions>>().Value);

app.Services.GetRequiredService<ILogger<Program>>()
    .LogInformation("Identity service has started");

app.Run();