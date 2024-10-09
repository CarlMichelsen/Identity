using Domain.Abstraction;

namespace Interface.Repository;

public interface IUserLogoutRepository
{
    Task<Result> Logout(long loginId);
}