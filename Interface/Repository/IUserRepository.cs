using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Repository;

public interface IUserRepository
{
    Task<Result<UserEntity>> GetUser(long id);

    Task<Result<UserEntity>> GetUser(OAuthProvider provider, string providerId);

    Task<Result<UserEntity>> RegisterUser(IUserConvertible userConvertible, string accessToken);
}