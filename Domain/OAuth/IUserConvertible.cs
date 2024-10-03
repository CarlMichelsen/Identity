using Domain.Abstraction;

namespace Domain.OAuth;

public interface IUserConvertible
{
    Result<OAuthUser> ToUser();
}