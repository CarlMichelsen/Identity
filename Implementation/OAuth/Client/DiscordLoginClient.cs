using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Domain.OAuth.Discord;
using Interface.OAuth;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.OAuth.Client;

public class DiscordLoginClient(
    IOAuthLoginAccessControl accessControl,
    IOptions<OAuthOptions> oAuthOptions,
    HttpClient httpClient) : IOAuthClient
{
    private const string AccessTokenPath = "/api/oauth2/token";
    private const string UserPath = "/api/users/@me";
    
    public async Task<Result<Uri>> CreateOAuthRedirect(LoginRedirectInformation loginRedirectInformation)
    {
        try
        {
            var processContextResult = await accessControl.GetProcessIdentifier(loginRedirectInformation);
            if (processContextResult.IsError)
            {
                return processContextResult.Error!;
            }

            var processContext = processContextResult.Unwrap();
            var scopes = new List<string> { "identify", "email", };
            return new OAuthUriBuilder(new Uri(oAuthOptions.Value.Discord.OAuthEndpoint))
                .SetQueryParameter("response_type", "code")
                .SetQueryParameter("client_id", oAuthOptions.Value.Discord.ClientId)
                .SetQueryParameter("redirect_uri", oAuthOptions.Value.Github.OAuthReturnEndpoint)
                .SetQueryParameter("scope", string.Join(' ', scopes))
                .SetQueryParameter("state", processContext.Identifier.State.ToString())
                .SetQueryParameter("prompt", "consent")
                .GetUrl();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "An exception was thrown during login completion",
                e);
        }
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

    private async Task<Result<DiscordCodeDto>> ExchangeCode(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", oAuthOptions.Value.Discord.ClientId },
            { "client_secret", oAuthOptions.Value.Discord.ClientSecret },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", oAuthOptions.Value.Discord.OAuthReturnEndpoint },
        });
        
        var response = await httpClient.PostAsync(
            AccessTokenPath,
            content);

        if (!response.IsSuccessStatusCode)
        {
            return new ResultError(
                ResultErrorType.HttpStatus,
                $"Code exchange response had a {response.StatusCode} status-code");
        }
        
        DiscordCodeDto? parsedJson;
        try
        {
            parsedJson = await response.Content
                .ReadFromJsonAsync<DiscordCodeDto>();
            if (parsedJson is null)
            {
                return new ResultError(
                    ResultErrorType.JsonParse,
                    "Code response was null");
            }
        }
        catch (JsonException e)
        {
            return new ResultError(
                ResultErrorType.JsonParse,
                await response.Content.ReadAsStringAsync(),
                e);
        }

        return parsedJson;
    }

    private async Task<Result<IUserConvertible>> GetUser(string accessToken)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(new Uri(oAuthOptions.Value.Github.OAuthEndpoint), UserPath),
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return new ResultError(
                ResultErrorType.HttpStatus,
                $"Code exchange response had a {response.StatusCode} status-code");
        }
        
        DiscordUserDto? parsedJson;
        try
        {
            parsedJson = await response.Content
                .ReadFromJsonAsync<DiscordUserDto>();
            if (parsedJson is null)
            {
                return new ResultError(
                    ResultErrorType.JsonParse,
                    "Code response was null");
            }
        }
        catch (JsonException e)
        {
            return new ResultError(
                ResultErrorType.JsonParse,
                await response.Content.ReadAsStringAsync(),
                e);
        }

        return parsedJson;
    }
}