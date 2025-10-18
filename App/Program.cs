using App;
using App.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterIdentityDependencies();

var app = builder.Build();

app.UseExceptionHandler(_ => {});

app.UseCors("whitelist");

app.MapOpenApiAndScalar();

app.MapHealthChecks("health");

app.MapControllers();

app.LogStartup();

app.Run();