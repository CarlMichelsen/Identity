using System.Text.Json;
using Microsoft.Extensions.Logging;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.LoginCallback;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.Test;

public class TestLoginReceiver(
    ILogger<TestLoginReceiver> logger) : ILoginReceiver
{
    public async Task<IAuthenticatedUserConvertible> GetAuthUser(Dictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue("code", out var code))
        {
            throw new OAuthException("code is missing");
        }
        
        var testUser = TestUserContainer.GetUsers.TryGetValue(code, out var user)
            ? await Task.FromResult<IAuthenticatedUserConvertible>(user)
            : throw new OAuthException($"Test-user with {nameof(TestUser.AuthenticationProviderId)} was not found");
        
        logger.LogInformation(
            "Test-user: {UserJson}",
            JsonSerializer.Serialize(testUser as TestUser));
        return testUser;
    }
}