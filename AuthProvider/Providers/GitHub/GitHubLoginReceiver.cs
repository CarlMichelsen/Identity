using Presentation.Service.OAuth.Login.Receive;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.GitHub;

public class GitHubLoginReceiver : ILoginReceiver
{
    public Task<IAuthenticatedUserConvertible> GetAuthUser(
        Dictionary<string, string> parameters)
    {
        throw new NotImplementedException();
    }
}