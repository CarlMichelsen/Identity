using Database;
using Database.Entity;
using Domain.Abstraction;

namespace Interface.Repository;

public interface IUserRefreshRepository
{
    Task<Result<(RefreshRecordEntity? NewRefreshRecord, AccessRecordEntity NewAccessRecord)>> Refresh(
        IClientInfo clientInfo,
        long refreshId);
}