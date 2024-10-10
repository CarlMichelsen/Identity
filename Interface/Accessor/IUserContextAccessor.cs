using Domain.Abstraction;
using Domain.User;

namespace Interface.Accessor;

public interface IUserContextAccessor
{
    Result<UserContext> GetUserContext();
}