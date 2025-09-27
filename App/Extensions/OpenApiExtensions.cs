using Scalar.AspNetCore;

namespace App.Extensions;

public static class OpenApiExtensions
{
    public static WebApplication MapOpenApiAndScalarForDevelopment(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app
            .MapOpenApi()
            .CacheOutput();

        app
            .MapScalarApiReference()
            .CacheOutput();

        return app;
    }
}