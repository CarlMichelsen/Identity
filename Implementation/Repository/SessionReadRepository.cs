using Database;
using Database.Entity;
using Domain.Abstraction;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class SessionReadRepository(
    ApplicationContext applicationContext) : ISessionReadRepository
{
    public async Task<Result<List<LoginRecordEntity>>> GetSessions(
        long userId)
    {
        try
        {
            return await applicationContext.LoginRecord
                .Where(lr => lr.UserId == userId && lr.InvalidatedUtc == null)
                .Include(l => l.RefreshRecords)
                .ThenInclude(rr => rr.AccessRecords)
                .AsSplitQuery()
                .ToListAsync();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while getting sessions",
                e);
        }
    }
}