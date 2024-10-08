using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class DevelopmentEndpoints
{
    public static void RegisterDevelopmentEndpoints(
        this IEndpointRouteBuilder apiGroup)
    {
        var developmentGroup = apiGroup
            .MapGroup("development")
            .WithTags("Development");

        developmentGroup.MapPost(
            "registerUser/{testUserId:long}",
            async ([FromServices] IDevelopmentLoginHandler handler, [FromRoute] long testUserId, [FromQuery] string state)
                => await handler.Login(testUserId, state));

        developmentGroup.MapGet(
            "users",
            async ([FromServices] IDevelopmentLoginHandler handler)
                => await handler.GetTestUsers());
    }
}