using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthProvider.Providers.GitHub.Model;
using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration;
using Presentation.Configuration.Options;
using Presentation.Service;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.LoginCallback;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.GitHub;

public class GitHubLoginReceiver(
    HttpClient httpClient,
    IOptionsSnapshot<AuthOptions> authOptions,
    IOAuthRedirectUriFactory redirectUriFactory) : ILoginReceiver
{
    private const string AccessTokenPath = "login/oauth/access_token";
    private const string UserPath = "user";
    
    public async Task<IAuthenticatedUserConvertible> GetAuthUser(
        Dictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue("code", out var code))
        {
            throw new OAuthException("Code query parameter was not present in redirect uri");
        }

        var codeResponse = await ExchangeCode(code);
        return await this.GetUser(codeResponse.AccessToken);
    }
    
    private async Task<GitHubCodeResponseDto> ExchangeCode(string code)
    {
        ArgumentNullException.ThrowIfNull(authOptions.Value.GitHub);
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", authOptions.Value.GitHub.ClientId },
            { "client_secret", authOptions.Value.GitHub.ClientSecret },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUriFactory.GetRedirectUri(AuthenticationProvider.GitHub).AbsoluteUri },
        });
        
        var response = await httpClient.PostAsync(
            new Uri(authOptions.Value.GitHub.ApiUrl, AccessTokenPath),
            content);

        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthException($"Code exchange response had a {response.StatusCode} status-code");
        }

        var parsedJson = await response.Content.ReadFromJsonAsync<GitHubCodeResponseDto>();
        return parsedJson ??  throw new OAuthException("GitHubCodeResponseDto response was null");
    }
    
    private async Task<IAuthenticatedUserConvertible> GetUser(string accessToken)
    {
        ArgumentNullException.ThrowIfNull(authOptions.Value.GitHub);
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(authOptions.Value.GitHub.UserUrl, UserPath),
        };
        
        request.Headers.UserAgent.Add(
            new ProductInfoHeaderValue(ApplicationConstants.Name, ApplicationConstants.Version));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthException($"User response had a {response.StatusCode} status-code");
        }

        var parsedJson = await response.Content.ReadFromJsonAsync<GitHubUserDto>();
        return parsedJson ?? throw new OAuthException("GitHubUserDto response was null");
    }
}