using Presentation.Service.OAuth.Model;

namespace Presentation.Service.OAuth.LoginCallback;

public interface ILoginReceiver
{
    Task<IAuthenticatedUserConvertible> GetAuthUser(Dictionary<string, string> parameters);
}