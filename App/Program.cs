using App;
using App.Endpoints;
using App.Extensions;
using App.Middleware;
using Database.Entity;
using Implementation.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterApplicationDependencies();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    // "We are all born ignorant, but one must work hard to remain stupid." - unknown
    await app.Services.EnsureDatabaseUpdated();
}

app.UseMiddleware<UnhandledExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<EndpointLogMiddleware>();
    app.UseCors(ApplicationConstants.DevelopmentCorsPolicyName);
}
else
{
    app.UseCors(ApplicationConstants.ProductionCorsPolicyName);
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
});

// OpenApi and Scalar endpoints - only enabled in development mode
app.MapOpenApiAndScalarForDevelopment();

app.UseAuthentication();

app.UseAuthorization();

var apiGroup = app.MapGroup("api/v1").RequireAuthorization();

apiGroup.RegisterLoginEndpoints();

apiGroup.RegisterUserEndpoints();

apiGroup.RegisterSessionEndpoints();

var oAuthOptions = app.Services.GetRequiredService<IOptions<OAuthOptions>>().Value;
if (oAuthOptions.Development is not null)
{
    apiGroup.RegisterDevelopmentEndpoints();
    app.UseStaticFiles(StaticFileOptionsFactory.Create());
    app.MapFallbackToFile("index.html");
}

app.MapGet("health", () => Results.Ok());

app.LogStartup();

app.Run();