using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Microsoft.AspNetCore.Http;

namespace Implementation.Util;

public static class LoginContextRetriever
{
    public static Result<LoginProcessIdentifier> GetLoginProcessIdentifier(Guid state, HttpContext? context)
    {
        if (context is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No available http context");
        }

        var ipResult = IpRetriever.GetIp(context);
        var userAgent = context.Request.Headers.UserAgent.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Failed to get user-agent from request");
        }
        
        return new Domain.OAuth.LoginProcessIdentifier(
            State: state,
            Ip: ipResult.Unwrap(),
            UserAgent: userAgent);
    }

    public static Result<(Guid State, string Code)> GetStateAndCodeFromQuery(
        Dictionary<string, string> queryParameters)
    {
        if (!queryParameters.TryGetValue("code", out var code))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No exchange code found");
        }
            
        if (!queryParameters.TryGetValue("state", out var stateString))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No state found");
        }

        if (!Guid.TryParse(stateString, out var state))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Invalid state");
        }

        return (State: state, Code: code);
    }
}