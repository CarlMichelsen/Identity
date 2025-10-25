using Presentation.Service.OAuth.Login.Receive;
using Presentation.Service.OAuth.Model;

namespace AuthProvider.Providers.Discord;

public class DiscordLoginReceiver : ILoginReceiver
{
    public Task<IAuthenticatedUserConvertible> GetAuthUser(Dictionary<string, string> parameters)
    {
        throw new NotImplementedException();
    }
}