using Test.Integration.Util;

namespace Test.Integration;

public class HealthCheckTest(IdentityWebApplicationFactory factory) : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly HttpClient client = factory.CreateClient();
    
    [Fact]
    public async Task Status_Endpoint_Returns_Success()
    {
        // Act
        var response = await client.GetAsync("/health", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}