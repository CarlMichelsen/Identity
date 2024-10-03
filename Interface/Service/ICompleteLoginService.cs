using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Service;

public interface ICompleteLoginService
{
    Task<Result<LoginProcessContext>> CompleteLogin(OAuthProvider provider);
}