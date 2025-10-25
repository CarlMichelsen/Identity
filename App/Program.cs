using App;
using App.Extensions;
using AuthProvider.Providers.Test;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterIdentityDependencies();

var app = builder.Build();

app.UseCors("whitelist");

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.MapGet("/api/v1/test-users", () => TestUserContainer.GetUsers);
    app.MapOpenApiAndScalar();
}

app.UseGlobalExceptionHandler(app.Logger);

app.MapHealthChecks("health");

app.MapControllers();

app.Run();