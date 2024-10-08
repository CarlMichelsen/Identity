using Domain.Configuration;
using Domain.OAuth;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class LoginEndpoints
{
    public static void RegisterLoginEndpoints(
        this IEndpointRouteBuilder apiGroup)
    {
        var loginGroup = apiGroup
            .MapGroup("login")
            .WithTags("Login");
        
        loginGroup.MapGet(
            "{authenticationMethod}",
            async ([FromRoute] string authenticationMethod, [FromQuery] string dest, [FromServices] IOAuthRedirectHandler handler) =>
                await handler.CreateOAuthRedirect(authenticationMethod, dest));
        
        loginGroup.MapGet(
            "complete/{authenticationMethod}",
            async ([FromServices] ICompleteLoginHandler handler, [FromRoute] string authenticationMethod) =>
            await handler.CompleteLogin(authenticationMethod));
    }
}