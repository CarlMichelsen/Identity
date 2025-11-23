using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthProvider.Providers.Discord.model;
using AuthProvider.Providers.Discord.Model;
using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration;
using Presentation.Configuration.Options;
using Presentation.Service;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.LoginCallback;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.Discord;

public class DiscordLoginReceiver(
    IOptionsSnapshot<AuthOptions> authOptions,
    IOAuthRedirectUriFactory redirectUriFactory,
    HttpClient httpClient) : ILoginReceiver
{
    private const string AccessTokenPath = "/api/oauth2/token";
    private const string UserPath = "/api/users/@me";
    
    public async Task<IAuthenticatedUserConvertible> GetAuthUser(Dictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue("code", out var code))
        {
            throw new OAuthException("Code query parameter was not present in redirect uri");
        }

        var codeResponse = await ExchangeCode(code);
        return await this.GetUser(codeResponse.AccessToken);
    }
    
    private async Task<DiscordCodeDto> ExchangeCode(string code)
    {
        ArgumentNullException.ThrowIfNull(authOptions.Value.Discord);
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", authOptions.Value.Discord.ClientId },
            { "client_secret", authOptions.Value.Discord.ClientSecret },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUriFactory.GetRedirectUri(AuthenticationProvider.Discord).AbsoluteUri },
        });
        
        var response = await httpClient.PostAsync(
            AccessTokenPath,
            content);

        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthException($"Code exchange response had a {response.StatusCode} status-code");
        }
        
        var parsedJson = await response.Content.ReadFromJsonAsync<DiscordCodeDto>();
        return parsedJson ?? throw new OAuthException("Code response was null");
    }
    
    private async Task<IAuthenticatedUserConvertible> GetUser(string accessToken)
    {
        ArgumentNullException.ThrowIfNull(authOptions.Value.Discord);
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(authOptions.Value.Discord.OAuthEndpoint, UserPath),
        };
        
        request.Headers.UserAgent.Add(
            new ProductInfoHeaderValue(ApplicationConstants.Name, ApplicationConstants.Version));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthException($"Code exchange response had a {response.StatusCode} status-code");
        }
        
        var parsedJson = await response.Content .ReadFromJsonAsync<DiscordUserDto>();
        return parsedJson ?? throw new OAuthException("DiscordUserDto response was null");
    }
}