using System.Net;
using Shouldly;
using Test.Integration.Util;

namespace Test.Integration;

public class HealthCheckTest(IdentityWebApplicationFactory factory)
    : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly HttpClient client = factory.CreateClient();
    
    [Fact]
    public async Task Health_Endpoint_Returns_Success()
    {
        // Arrange
        // Act
        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}