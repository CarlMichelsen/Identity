namespace App.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(
        this IEndpointRouteBuilder apiGroup)
    {
        var loginGroup = apiGroup
            .MapGroup("user")
            .WithTags("User");

        loginGroup.MapGet(string.Empty, () => Results.Ok());
        
        loginGroup.MapPut(string.Empty, () => Results.Ok());
    }
}