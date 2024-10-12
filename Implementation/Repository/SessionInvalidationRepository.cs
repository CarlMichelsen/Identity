using Database;
using Database.Entity;
using Domain.Abstraction;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class SessionInvalidationRepository(
    ApplicationContext applicationContext) : ISessionInvalidationRepository
{
    public async Task<Result<List<LoginRecordEntity>>> InvalidateSessions(
        long userId,
        List<long> loginIds)
    {
        await using var transaction = await applicationContext.Database.BeginTransactionAsync();
        try
        {
            var logins = await applicationContext.LoginRecord
                .Where(lr => lr.UserId == userId && loginIds.Contains(lr.Id))
                .Include(l => l.RefreshRecords)
                .ThenInclude(rr => rr.AccessRecords)
                .AsSplitQuery()
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var login in logins)
            {
                login.InvalidatedUtc ??= now;
                login.RefreshRecords.ForEach(rr => { rr.InvalidatedUtc ??= now; });
            }
            
            await applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return logins;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while invalidating user sessions",
                e);
        }
    }
}