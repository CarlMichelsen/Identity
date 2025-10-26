using System.Net;
using Application.Service.OAuth;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Test.Integration.Util;

namespace Test.Integration;

public class LoginRedirectTest(IdentityWebApplicationFactory factory)
    : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = factory.ClientOptions.BaseAddress,
        AllowAutoRedirect = false,
        MaxAutomaticRedirections = 0,
        HandleCookies = factory.ClientOptions.HandleCookies,
    });
    
    [Fact]
    public async Task TestLoginCreatesAProcess()
    {
        // Arrange
        var successUri = factory.Server.BaseAddress.AbsoluteUri + "success";
        var errorUri = factory.Server.BaseAddress.AbsoluteUri + "error";
        var uri = new OAuthUriBuilder(factory.Server.BaseAddress)
            .SetPath("/api/v1/Login/Test")
            .AddQueryParam("SuccessRedirectUrl", Uri.EscapeDataString(successUri))
            .AddQueryParam("ErrorRedirectUrl", Uri.EscapeDataString(errorUri))
            .Build();
        
        // Act
        var response = await client.GetAsync(uri, CancellationToken.None);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        var found = response.Headers.TryGetValues("Location", out var locationHeader);
        found.ShouldBeTrue();
        var list = locationHeader?.ToList();
        list.ShouldNotBeNull();
        list.ShouldNotBeEmpty();
        list.First().ShouldBe(successUri);
    }
}