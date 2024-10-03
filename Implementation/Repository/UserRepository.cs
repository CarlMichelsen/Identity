using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;
using Interface.Repository;

namespace Implementation.Repository;

public class UserRepository : IUserRepository
{
    public Task<Result<UserEntity>> GetUser(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserEntity>> GetUser(OAuthProvider provider, string providerId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserEntity>> RegisterUser(IUserConvertible userConvertible, string accessToken)
    {
        throw new NotImplementedException();
    }
}