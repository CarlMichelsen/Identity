using Presentation.Service.OAuth.Model;

namespace Presentation.Service.OAuth.Login.Receive;

public interface ILoginReceiver
{
    Task<IAuthenticatedUserConvertible> GetAuthUser(Dictionary<string, string> parameters);
}