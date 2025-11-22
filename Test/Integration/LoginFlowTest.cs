using System.Net;
using AuthProvider.Providers.Test;
using Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Shouldly;
using Test.Fakes;
using Test.Integration.Util;

namespace Test.Integration;

public class LoginFlowTest
{
    [Fact]
    public async Task SimpleLoginFlow_ShouldWorkCorrectly()
    {
        // Arrange - Set up fake time provider starting at a specific time
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero));
        
        // Create a new factory instance with the custom time provider
        await using var customFactory = new IdentityWebApplicationFactory();
        customFactory.CustomTimeProvider = fakeTimeProvider;

        // Get the actual server URL to update configuration
        var serverUrl = customFactory.Server.BaseAddress.AbsoluteUri.TrimEnd('/');
        
        // Create a new factory with updated configuration
        await using var factory = customFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Auth:Self"] = serverUrl,
                    ["Auth:Test:OAuthEndpoint"] = $"{serverUrl}/testprovider.html",
                }!);
            });
        });
        
        // Create a new client with cookie handling enabled
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = factory.ClientOptions.BaseAddress,
            AllowAutoRedirect = false,
            MaxAutomaticRedirections = 0,
            HandleCookies = true,
        });
        
        using var scope = factory.Services.CreateScope();
        var authOptions = scope
            .ServiceProvider
            .GetRequiredService<IOptionsSnapshot<AuthOptions>>()
            .Value;
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        // Step 1: Initiate login flow
        var successUri = factory.Server.BaseAddress!.AbsoluteUri + "success";
        var errorUri = factory.Server.BaseAddress.AbsoluteUri + "error";
        var loginUri = $"/api/v1/Login/Test?SuccessRedirectUrl={Uri.EscapeDataString(successUri)}&ErrorRedirectUrl={Uri.EscapeDataString(errorUri)}";
        
        var loginResponse = await client.GetAsync(loginUri, TestContext.Current.CancellationToken);
        
        // Assert login redirect
        loginResponse.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        var loginRedirectLocation = loginResponse.Headers.Location?.ToString();
        loginRedirectLocation.ShouldNotBeNull();
        loginRedirectLocation.ShouldStartWith(serverUrl + "/testprovider.html");
        
        // Extract state parameter from redirect
        var stateParam = ExtractQueryParameter(loginRedirectLocation, "state");
        stateParam.ShouldNotBeNullOrWhiteSpace();
        
        // Step 2: Complete OAuth flow
        var testUser = TestUserContainer.Steve;
        var authorizeUri = $"/api/v1/OAuth/Authorize/Test?state={stateParam}&code={testUser.AuthenticationProviderId}";
        
        var authorizeResponse = await client.GetAsync(authorizeUri, TestContext.Current.CancellationToken);
        
        // Assert successful authorization
        authorizeResponse.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        var authorizeRedirectLocation = authorizeResponse.Headers.Location?.ToString();
        authorizeRedirectLocation.ShouldNotBeNull();
        authorizeRedirectLocation.ShouldStartWith(successUri);
        
        // Verify cookies were set
        var cookies = GetCookiesFromResponse(authorizeResponse);
        cookies.Keys.ShouldContain(authOptions.AccessToken.CookieName);
        cookies.Keys.ShouldContain(authOptions.RefreshToken.CookieName);
        
        // Verify database state
        var users = await context.User.ToListAsync(TestContext.Current.CancellationToken);
        users.Count.ShouldBe(1);
        users[0].Username.ShouldBe(testUser.Username);
        
        var logins = await context.Login.ToListAsync(TestContext.Current.CancellationToken);
        logins.Count.ShouldBe(1);
        logins[0].CreatedAt.ShouldBe(fakeTimeProvider.GetUtcNow().UtcDateTime);
        
        // Step 3: Test refresh - advance time by 5 minutes
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(5));
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Auth/Refresh");
        var refreshToken = cookies[authOptions.RefreshToken.CookieName];
        var accessToken = cookies[authOptions.AccessToken.CookieName];
        request.Headers.Add("Cookie", $"{authOptions.AccessToken.CookieName}={accessToken};{authOptions.RefreshToken.CookieName}={refreshToken}");

        var refreshResponse = await client.SendAsync(request, TestContext.Current.CancellationToken);
        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    
    private static string? ExtractQueryParameter(string url, string paramName)
    {
        var uri = new Uri(url);
        var query = uri.Query.TrimStart('?');
        var pairs = query.Split('&');
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length == 2 && parts[0] == paramName)
            {
                return Uri.UnescapeDataString(parts[1]);
            }
        }
        
        return null;
    }
    
    private static Dictionary<string, string> GetCookiesFromResponse(HttpResponseMessage response)
    {
        var cookies = new Dictionary<string, string>();
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
        {
            return cookies;
        }
        
        foreach (var header in cookieHeaders)
        {
            // Split by semicolon to get the name=value pair (first part)
            var parts = header.Split(';');
            if (parts.Length <= 0)
            {
                continue;
            }
            
            var nameValue = parts[0].Trim();
            var separatorIndex = nameValue.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }
            
            var name = nameValue[..separatorIndex];
            var value = nameValue[(separatorIndex + 1)..];
            cookies[name] = value;
        }
        return cookies;
    }
}
