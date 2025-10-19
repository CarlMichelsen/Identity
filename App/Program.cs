using App;
using App.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterIdentityDependencies();

var app = builder.Build();

app.UseCors("whitelist");

app.MapOpenApiAndScalar();

app.UseGlobalExceptionHandler(app.Logger);

app.MapHealthChecks("health");

app.MapControllers();

app.Run();