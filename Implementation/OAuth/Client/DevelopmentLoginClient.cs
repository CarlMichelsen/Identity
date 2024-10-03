using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Domain.OAuth.Development;
using Interface.OAuth;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.OAuth.Client;

public sealed class DevelopmentLoginClient(
    IOAuthLoginAccessControl accessControl,
    IDevelopmentUserService developmentUserService,
    IOptions<OAuthOptions> oAuthOptions) : IOAuthClient
{
    public async Task<Result<Uri>> CreateOAuthRedirect(LoginRedirectInformation loginRedirectInformation)
    {
        if (oAuthOptions.Value.Development is null)
        {
            throw new Exception("Can't do development login if there is no configuration");
        }
        
        var processContextResult = await accessControl.GetProcessIdentifier(loginRedirectInformation);
        if (processContextResult.IsError)
        {
            return processContextResult.Error!;
        }

        var processContext = processContextResult.Unwrap();
        return new OAuthUriBuilder(new Uri(oAuthOptions.Value.Development.OAuthEndpoint), true)
            .SetQueryParameter("response_type", "code")
            .SetQueryParameter("client_id", oAuthOptions.Value.Development.ClientId)
            .SetQueryParameter("redirect_uri", oAuthOptions.Value.Development.OAuthReturnEndpoint)
            .SetQueryParameter("state", processContext.Identifier.State.ToString())
            .GetUrl();
    }

    public async Task<Result<LoginProcessContext>> CompleteLogin(Dictionary<string, string> queryParameters)
    {
        try
        {
            var loginProcessResult = await accessControl.ValidateLoginProcess(
                queryParameters,
                LoginContextRetriever.GetStateAndCodeFromQuery, 
                async code =>
                {
                    var codeResponseResult = await this.ExchangeCode(code);
                    if (codeResponseResult.IsError)
                    {
                        return codeResponseResult.Error!;
                    }
                
                    return await this.GetUser(codeResponseResult.Unwrap().AccessToken);
                });

            if (loginProcessResult.IsError)
            {
                return loginProcessResult.Error!;
            }

            return loginProcessResult.Unwrap();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "An exception was thrown during login completion",
                e);
        }
    }
    
    private Task<Result<DevelopmentCodeDto>> ExchangeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Task.FromResult<Result<DevelopmentCodeDto>>(new ResultError(
                ResultErrorType.MapError,
                "Code is null or whitespace"));
        }
        
        var codeResponse = new DevelopmentCodeDto
        {
            AccessToken = code,
        };
        return Task.FromResult<Result<DevelopmentCodeDto>>(codeResponse);
    }

    private async Task<Result<IUserConvertible>> GetUser(string accessToken)
    {
        var developmentUserResult = await developmentUserService
            .GetDevelopmentUserFromAccessToken(accessToken);
        if (developmentUserResult.IsError)
        {
            return developmentUserResult.Error!;
        }

        return developmentUserResult.Unwrap();
    }
}