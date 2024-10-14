using Domain.Dto;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Handler;

public class UserRefreshHandler(
    IErrorLogService errorLogService,
    IUserRefreshService userRefreshService,
    IUserLogoutService userLogoutService) : IUserRefreshHandler
{
    public async Task<IResult> Refresh()
    {
        var refreshResult = await userRefreshService.Refresh();
        if (refreshResult.IsError)
        {
            errorLogService.Log(refreshResult.Error!);
            return Results.Ok(new ServiceResponse("failure"));
        }
        
        return Results.Ok(refreshResult.Unwrap());
    }

    public async Task<IResult> Logout()
    {
        var logoutResult = await userLogoutService.Logout();
        if (logoutResult.IsError)
        {
            errorLogService.Log(logoutResult.Error!);
            return Results.Ok(new ServiceResponse("failure"));
        }

        return Results.Ok(logoutResult.Unwrap());
    }
}