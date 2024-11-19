using Domain.Dto;
using Domain.User;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(
        this IEndpointRouteBuilder apiGroup)
    {
        var loginGroup = apiGroup
            .MapGroup("user")
            .WithTags("User");

        loginGroup.MapGet(
            string.Empty,
            ([FromServices] IUserReadHandler handler) => handler.GetUser())
            .Produces<ServiceResponse<AuthenticatedUser>>()
            .AllowAnonymous();
        
        loginGroup.MapDelete(
            string.Empty,
            ([FromServices] IUserRefreshHandler handler) => handler.Logout())
            .Produces<ServiceResponse>();
        
        loginGroup.MapPut(
                string.Empty,
                ([FromServices] IUserRefreshHandler handler) => handler.Refresh())
            .Produces<ServiceResponse>()
            .AllowAnonymous();
    }
}