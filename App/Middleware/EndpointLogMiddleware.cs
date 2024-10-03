namespace App.Middleware;

public class EndpointLogMiddleware(
    ILogger<EndpointLogMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        logger.LogInformation(
            "{Method} {Path}",
            context.Request.Method,
            context.Request.Path);
        await next(context);
    }
}