using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class UserReadHandler(
    IErrorLogService errorLogService,
    IUserReadService userReadService) : IUserReadHandler
{
    public IResult GetUser()
    {
        var userResult = userReadService.GetUser();
        if (userResult.IsError)
        {
            errorLogService.Log(userResult.Error!);
            return Results.StatusCode(500);
        }

        return Results.Ok(userResult.Unwrap());
    }
}