using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace App.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                if (feature is null) return;

                var exception = feature.Error;

                var problem = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/500",
                    Title = "An unexpected error occurred.",
                    Detail = exception.Message,
                    Status = (int)HttpStatusCode.InternalServerError,
                    Instance = context.Request.Path
                };

                // You can customize further based on exception type:
                switch (exception)
                {
                    case KeyNotFoundException:
                        problem.Status = (int)HttpStatusCode.NotFound;
                        problem.Title = "Resource not found.";
                        problem.Type = "https://httpstatuses.com/404";
                        break;

                    case UnauthorizedAccessException:
                        problem.Status = (int)HttpStatusCode.Unauthorized;
                        problem.Title = "Unauthorized.";
                        problem.Type = "https://httpstatuses.com/401";
                        break;

                    case ArgumentException:
                        problem.Status = (int)HttpStatusCode.BadRequest;
                        problem.Title = "Invalid request.";
                        problem.Type = "https://httpstatuses.com/400";
                        break;
                }

                logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problem.Status ?? 500;

                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }
}