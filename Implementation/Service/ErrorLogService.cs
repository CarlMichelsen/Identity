using Domain.Abstraction;
using Implementation.Configuration;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Implementation.Service;

public class ErrorLogService(
    IHttpContextAccessor contextAccessor,
    ILogger<ErrorLogService> logger) : IErrorLogService
{
    public string Log(ResultError error)
    {
        var traceId = this.GetTraceId() ?? Guid.NewGuid().ToString();
        if (error.InnerException is null)
        {
            logger.LogInformation(
                "ResultError [{TraceId}] |{Type}| {Description}",
                traceId,
                Enum.GetName(error.Type) ?? "unknown",
                error.Description);

            return traceId;
        }
        
        logger.LogCritical(
            error.InnerException,
            "ResultError [{TraceId}] |{Type}| {Description}",
            traceId,
            Enum.GetName(error.Type) ?? "unknown",
            error.Description);
            
        return traceId;
    }
    
    private string? GetTraceId()
    {
        return contextAccessor.HttpContext?.Response.Headers
            .TryGetValue(ApplicationConstants.TraceIdHeaderName, out var traceIdValues) == true
                ? traceIdValues.FirstOrDefault()
                : default;
    }
}