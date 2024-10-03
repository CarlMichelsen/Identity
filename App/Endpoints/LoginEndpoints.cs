using System.Security.Cryptography;
using Domain.Configuration;
using Domain.OAuth;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class LoginEndpoints
{
    public static void RegisterLoginEndpoints(
        this IEndpointRouteBuilder apiGroup,
        FeatureFlagOptions featureFlag)
    {
        var loginGroup = apiGroup
            .MapGroup("login")
            .WithTags("Login");
        
        loginGroup.MapGet(
            "{authenticationMethod}",
            async ([FromRoute] string authenticationMethod, [FromQuery] string dest, [FromServices] IOAuthRedirectHandler handler) =>
                await handler.CreateOAuthRedirect(authenticationMethod, dest));

        var completeGroup = loginGroup.MapGroup("complete");
        if (featureFlag.DevelopmentLoginEnabled)
        {
            completeGroup.MapGet(
                "development",
                async ([FromServices] ICompleteLoginHandler handler) =>
                    await handler.CompleteLogin(OAuthProvider.Development));
        }
        
        completeGroup.MapGet(
            "discord",
            async ([FromServices] ICompleteLoginHandler handler) =>
                await handler.CompleteLogin(OAuthProvider.Discord));
        
        completeGroup.MapGet(
            "github",
            async ([FromServices] ICompleteLoginHandler handler) =>
                await handler.CompleteLogin(OAuthProvider.Github));
        
        completeGroup.MapGet(
            "guest",
            async ([FromServices] ICompleteLoginHandler handler) =>
                await handler.CompleteLogin(OAuthProvider.Guest));
    }
}