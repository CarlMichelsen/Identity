using Domain.Dto;
using Domain.OAuth.Development;
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
            .AllowAnonymous()
            .WithTags("Development");

        developmentGroup.MapPost(
            "registerUser/{testUserId:long}",
            async ([FromServices] IDevelopmentLoginHandler handler, [FromRoute] long testUserId, [FromQuery] string state)
                => await handler.Login(testUserId, state))
            .Produces<ServiceResponse<DevelopmentLoginResponse>>();

        developmentGroup.MapGet(
            "users",
            async ([FromServices] IDevelopmentLoginHandler handler)
                => await handler.GetTestUsers())
            .Produces<ServiceResponse<List<DevelopmentUserDto>>>();
    }
}