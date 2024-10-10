using Database.Entity;
using Domain.Abstraction;

namespace Interface.Repository;

public interface ISessionInvalidationRepository
{
    Task<Result<List<LoginRecordEntity>>> InvalidateSessions(long userId, List<long> loginIds);
}