using Database.Entity;
using Domain.Abstraction;

namespace Interface.Repository;

public interface ISessionReadRepository
{
    Task<Result<List<LoginRecordEntity>>> GetSessions(long userId);
}