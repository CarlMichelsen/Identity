using System.Net;
using Application.Service.OAuth;
using Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        using var scope = factory.Services.CreateScope();
        var successUri = factory.Server.BaseAddress.AbsoluteUri + "success"; // this is the final redirect.
        var errorUri = factory.Server.BaseAddress.AbsoluteUri + "error";
        var uri = new OAuthUriBuilder(factory.Server.BaseAddress)
            .SetPath("/api/v1/Auth/Login/Test")
            .AddQueryParam("SuccessRedirectUrl", Uri.EscapeDataString(successUri))
            .AddQueryParam("ErrorRedirectUrl", Uri.EscapeDataString(errorUri))
            .Build();
        
        // Act
        var response = await client.GetAsync(uri, TestContext.Current.CancellationToken);
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        // Get newest created OAuthProcess from the database
        var oAuthProcesses = await context
            .OAuthProcess
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        var found = response.Headers.TryGetValues("Location", out var locationHeader);
        found.ShouldBeTrue();
        var list = locationHeader?.ToList();
        list.ShouldNotBeNull();
        list.ShouldNotBeEmpty();
        list.First().ShouldStartWith("http://localhost:5220/testprovider.html");
        
        oAuthProcesses.ShouldNotBeEmpty();
        oAuthProcesses.First().SuccessRedirectUri.ShouldBe(new Uri(successUri));
        oAuthProcesses.First().ErrorRedirectUri.ShouldBe(new Uri(errorUri));
        oAuthProcesses.First().LoginRedirectUri.ShouldBe(new Uri(list.First()));
    }
}