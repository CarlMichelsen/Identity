using Database;
using Database.Entity;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class UserRefreshRepository(
    ApplicationContext applicationContext) : IUserRefreshRepository
{
    public async Task<Result<(RefreshRecordEntity? NewRefreshRecord, AccessRecordEntity NewAccessRecord)>> Refresh(
        IClientInfo clientInfo,
        long refreshId)
    {
        await using var transaction = await applicationContext.Database.BeginTransactionAsync();
        try
        {
            var existingRefreshRecord = await applicationContext.RefreshRecord
                .Include(rr => rr.LoginRecord)
                .Include(rr => rr.AccessRecords)
                .AsSplitQuery()
                .FirstOrDefaultAsync(rr => rr.Id == refreshId);

            if (existingRefreshRecord is null)
            {
                return new ResultError(
                    ResultErrorType.NotFound,
                    "Did not find refresh-token");
            }

            if (existingRefreshRecord.InvalidatedUtc is not null)
            {
                return new ResultError(
                    ResultErrorType.Unauthorized,
                    "RefreshRecord has been invalidated in the database");
            }
            
            if (existingRefreshRecord.LoginRecord.InvalidatedUtc is not null)
            {
                return new ResultError(
                    ResultErrorType.Unauthorized,
                    "Login has been invalidated in the database");
            }

            RefreshRecordEntity actualRefreshRecord;
            var timeToExpiry = existingRefreshRecord.ExpiresUtc - DateTime.UtcNow;
            var createNewRefreshRecord = timeToExpiry < TimeSpan.FromDays(14);
            if (createNewRefreshRecord)
            {
                existingRefreshRecord.InvalidatedUtc = DateTime.UtcNow;
                var loginRecord = existingRefreshRecord.LoginRecord;
                actualRefreshRecord = new RefreshRecordEntity
                {
                    LoginRecordId = loginRecord.Id,
                    LoginRecord = loginRecord,
                    ExpiresUtc = DateTime.UtcNow + ApplicationConstants.RefreshTokenLifeTime,
                    AccessRecords = [],
                    CreatedUtc = DateTime.UtcNow,
                    UserAgent = clientInfo.UserAgent,
                    Ip = clientInfo.Ip,
                };
                
                loginRecord.RefreshRecords.Add(existingRefreshRecord);
                await applicationContext.SaveChangesAsync();
            }
            else
            {
                actualRefreshRecord = existingRefreshRecord;
            }

            var accessRecord = new AccessRecordEntity
            {
                RefreshRecordId = actualRefreshRecord.Id,
                RefreshRecord = actualRefreshRecord,
                ExpiresUtc = DateTime.UtcNow + ApplicationConstants.AccessTokenLifeTime,
                CreatedUtc = DateTime.UtcNow,
                UserAgent = clientInfo.UserAgent,
                Ip = clientInfo.Ip,
            };
            
            actualRefreshRecord.AccessRecords.Add(accessRecord);
            await applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return (
                NewRefreshRecord: createNewRefreshRecord ? actualRefreshRecord : default,
                NewAccessRecord: accessRecord);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while refreshing user",
                e);
        }
    }
}