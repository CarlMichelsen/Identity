using Database;
using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Repository;

public interface IUserLoginRepository
{
    Task<Result<PostLoginData>> LoginUser(
        IOAuthUserConvertible userConvertible,
        IClientInfo clientInfo);
}