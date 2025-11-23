using Database.Entity;

namespace Presentation.Service.OAuth.LoginCallback;

public interface IFirstLoginNotifier
{
    void NotifyFirstLogin(UserEntity userEntity);
}