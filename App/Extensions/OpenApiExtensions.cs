using Scalar.AspNetCore;

namespace App.Extensions;

public static class OpenApiExtensions
{
    public static WebApplication MapOpenApiAndScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        return app;
    }
}
