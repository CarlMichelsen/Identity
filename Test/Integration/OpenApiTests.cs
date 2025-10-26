using System.Net;
using Shouldly;
using Test.Integration.Util;

namespace Test.Integration;

public class OpenApiTests(IdentityWebApplicationFactory factory)
    : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly HttpClient client = factory.CreateClient();
    
    [Fact]
    public async Task OpenApiTest()
    {
        // Arrange
        // Act
        var response = await client.GetAsync("/openapi/v1.json", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}