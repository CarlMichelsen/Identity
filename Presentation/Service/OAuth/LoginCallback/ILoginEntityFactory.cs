using Database.Entity;
using Presentation.Service.OAuth.Model;

namespace Presentation.Service.OAuth.LoginCallback;

/// <summary>
/// A factory that instantiates <see cref="LoginEntity" /> and connects it to <see cref="OAuthProcessEntity" />.
/// </summary>
public interface ILoginEntityFactory
{
    Task<LoginEntity> CreateLogin(
        AuthenticatedUser user,
        OAuthProcessEntity process);
}