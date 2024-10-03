using Domain.Configuration;

namespace App.Middleware;

public class UnhandledExceptionMiddleware(
    ILogger<UnhandledExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            this.EnsureTraceIdIsApplied(context);
            await next(context);
        }
        catch (Exception e)
        {
            var traceId = GetTraceId(context);
            logger.LogCritical(
                e,
                "Unhandled exception TraceId: {TraceId}",
                traceId);
            
            context.Response.StatusCode = 500;
        }
    }

    private static string? GetTraceId(HttpContext context)
    {
        return context.Response.Headers.TryGetValue(ApplicationConstants.TraceIdHeaderName, out var traceIdValues)
            ? traceIdValues.FirstOrDefault()
            : default;
    }

    private void EnsureTraceIdIsApplied(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(ApplicationConstants.TraceIdHeaderName, out var value))
        {
            context.Response.Headers[ApplicationConstants.TraceIdHeaderName] = value;
        }
        else
        {
            var traceId = Guid.NewGuid().ToString();
            context.Response.Headers[ApplicationConstants.TraceIdHeaderName] = traceId;
            logger.LogDebug("Generated Trace ID: {TraceId}", traceId);
        }
    }
}