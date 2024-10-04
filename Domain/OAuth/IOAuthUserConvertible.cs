using Domain.Abstraction;

namespace Domain.OAuth;

public interface IOAuthUserConvertible
{
    Result<OAuthUser> ToUser();
}