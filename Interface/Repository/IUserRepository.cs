using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Repository;

public interface IUserRepository
{
    Task<Result<UserEntity>> GetUser(long id);

    Task<Result<UserEntity>> GetUser(
        OAuthProvider provider,
        string providerId);

    Task<Result<(UserEntity User, long LoginId, long RefreshId, bool FirstLogin)>> LoginUser(
        IOAuthUserConvertible userConvertible,
        IClientInfo clientInfo);
    
    Task<Result<RefreshRecordEntity>> RefreshUser(
        long loginId,
        OAuthUser oAuthUser,
        IClientInfo clientInfo);
}