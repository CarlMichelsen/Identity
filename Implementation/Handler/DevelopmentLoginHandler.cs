using Domain.Abstraction;
using Domain.Dto;
using Domain.OAuth;
using Domain.OAuth.Development;
using Implementation.Service;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.Handler;

public class DevelopmentLoginHandler(
    IOptions<Domain.Configuration.OAuthOptions> oAuthOptions,
    IErrorLogService errorLogService,
    IDevelopmentUserService developmentUserService) : IDevelopmentLoginHandler
{
    public async Task<IResult> Login(long developmentUserId, string state)
    {
        var accessTokenResult = await developmentUserService.RegisterUserAccessToken(developmentUserId);
        if (accessTokenResult.IsError)
        {
            errorLogService.Log(accessTokenResult.Error!);
            return Results.StatusCode(500);
        }

        try
        {
            var builder = new OAuthUriBuilder(new Uri(oAuthOptions.Value.Development!.OAuthReturnEndpoint), true)
                .SetQueryParameter("code", accessTokenResult.Unwrap())
                .SetQueryParameter("state", state);

            var res = new DevelopmentLoginResponse(builder.GetUrl().AbsoluteUri);
            return Results.Ok(new ServiceResponse<DevelopmentLoginResponse>(res));
        }
        catch (Exception e)
        {
            var err = new ResultError(
                ResultErrorType.Exception,
                "Failed to construct development redirect uri",
                e);
            
            errorLogService.Log(err);
            return Results.StatusCode(500);
        }
    }

    public async Task<IResult> GetTestUsers()
    {
        return await Task.Run(() =>
        {
            var developmentUserList = DevelopmentUsers.Users
                .Select(kv => kv.Value)
                .ToList();
            var res = new ServiceResponse<List<DevelopmentUserDto>>(developmentUserList);
            return Results.Ok(res);
        });
    }
}