using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class SessionEndpoints
{
    public static void RegisterSessionEndpoints(
        this IEndpointRouteBuilder apiGroup)
    {
        var sessionGroup = apiGroup
            .MapGroup("session")
            .WithTags("Session");

        sessionGroup.MapGet(
            string.Empty,
            async ([FromServices] ISessionHandler handler) =>
                await handler.GetSessions());
        
        sessionGroup.MapDelete(
            "{commaSeparatedLongLoginIds}",
            async ([FromServices] ISessionHandler handler, [FromRoute] string commaSeparatedLongLoginIds) =>
                await handler.InvalidateSessions(commaSeparatedLongLoginIds));
    }
}