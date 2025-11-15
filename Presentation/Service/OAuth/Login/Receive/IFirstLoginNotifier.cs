using Database.Entity;

namespace Presentation.Service.OAuth.Login.Receive;

public interface IFirstLoginNotifier
{
    void NotifyFirstLogin(UserEntity userEntity);
}