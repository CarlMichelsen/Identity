using Database;
using Domain.Abstraction;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class UserLogoutRepository(
    ApplicationContext applicationContext) : IUserLogoutRepository
{
    public async Task<Result> Logout(long loginId)
    {
        try
        {
            var login = await applicationContext.LoginRecord
                .FindAsync(loginId);
            if (login is null)
            {
                return new ResultError(
                    ResultErrorType.NotFound,
                    "no login found");
            }

            await applicationContext.RefreshRecord
                .Where(rr => rr.LoginRecordId == login.Id)
                .ExecuteUpdateAsync(rr =>
                    rr.SetProperty(rrInner => rrInner.InvalidatedUtc, DateTime.UtcNow));
        
            login.InvalidatedUtc = DateTime.UtcNow;
            await applicationContext.SaveChangesAsync();
            return new Result();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                $"Exception occured during logout of id <{loginId}>",
                e);
        }
    }
}