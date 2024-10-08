using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Repository;

public interface IUserRepository
{
    Task<Result<UserEntity>> GetUser(long id);

    Task<Result<UserEntity>> GetUser(
        OAuthProvider provider,
        string providerId);

    Task<Result<PostLoginData>> LoginUser(
        IOAuthUserConvertible userConvertible,
        IClientInfo clientInfo);
}