using Implementation.Util;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class SessionHandler(
    IErrorLogService errorLogService,
    ISessionService sessionService) : ISessionHandler
{
    public async Task<IResult> GetSessions()
    {
        var sessionsResult = await sessionService.GetSessions();
        if (sessionsResult.IsError)
        {
            errorLogService.Log(sessionsResult.Error!);
            return Results.StatusCode(500);
        }

        return Results.Ok(sessionsResult.Unwrap());
    }

    public async Task<IResult> InvalidateSessions(string commaSeparatedLongLoginIds)
    {
        var idsResult = IdParser.Parse(commaSeparatedLongLoginIds);
        if (idsResult.IsError)
        {
            errorLogService.Log(idsResult.Error!);
            return Results.StatusCode(400);
        }

        var ids = idsResult.Unwrap();
        var invalidatedSessionResult = await sessionService.InvalidateSessions(ids);
        if (invalidatedSessionResult.IsError)
        {
            errorLogService.Log(invalidatedSessionResult.Error!);
            return Results.StatusCode(500);
        }

        return Results.Ok(invalidatedSessionResult.Unwrap());
    }
}