using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Domain.OAuth.Github;
using Interface.OAuth;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Implementation.OAuth.Client;

public class GithubLoginClient(
    IOAuthLoginAccessControl accessControl,
    IOptions<OAuthOptions> oAuthOptions,
    HttpClient httpClient) : IOAuthClient
{
    private const string AccessTokenPath = "login/oauth/access_token";
    private const string UserPath = "user";
    private const string UserSubdomain = "api";
    
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
            var scopes = new List<string> { "read:user", "user:email" };
            return new OAuthUriBuilder(new Uri(oAuthOptions.Value.Github.OAuthEndpoint))
                .SetQueryParameter("response_type", "token")
                .SetQueryParameter("client_id", oAuthOptions.Value.Github.ClientId)
                .SetQueryParameter("redirect_uri", oAuthOptions.Value.Github.OAuthReturnEndpoint)
                .SetQueryParameter("scope", string.Join(' ', scopes))
                .SetQueryParameter("state", processContext.Identifier.State.ToString())
                .SetQueryParameter("allow_signup", "false")
                .GetUrl();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "An exception was thrown during oAuth redirection",
                e);
        }
    }

    public async Task<Result<LoginProcessContext>> GetProviderLoginData(Dictionary<string, string> queryParameters)
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
                "An exception was thrown getting provider login data",
                e);
        }
    }

    private async Task<Result<GithubCodeResponseDto>> ExchangeCode(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", oAuthOptions.Value.Github.ClientId },
                { "client_secret", oAuthOptions.Value.Github.ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", oAuthOptions.Value.Github.OAuthReturnEndpoint },
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

        GithubCodeResponseDto? parsedJson;
        try
        {
            parsedJson = await response.Content
                .ReadFromJsonAsync<GithubCodeResponseDto>();
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

    private async Task<Result<IOAuthUserConvertible>> GetUser(string accessToken)
    {
        var baseUri = new Uri(oAuthOptions.Value.Github.OAuthEndpoint);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(new Uri($"https://{UserSubdomain}.{baseUri.Host}"), UserPath),
        };
        request.Headers.UserAgent.Add(
            new ProductInfoHeaderValue(ApplicationConstants.ApplicationName, "1.0"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return new ResultError(
                ResultErrorType.HttpStatus,
                $"User response had a {response.StatusCode} status-code");
        }

        GithubUserDto? parsedJson;
        try
        {
            parsedJson = await response.Content
                .ReadFromJsonAsync<GithubUserDto>();

            if (parsedJson is null)
            {
                return new ResultError(
                    ResultErrorType.JsonParse,
                    "User response was null");
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