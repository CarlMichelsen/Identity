using Database.Entity;
using Database.Entity.Id;

namespace Presentation.Service.OAuth.Refresh;

public interface ITokenRefreshPersistenceService
{
    Task<RefreshEntity?> GetRefreshEntity(RefreshEntityId refreshEntityId);
    
    (RefreshEntity Entity, string Jwt) CreateRefreshEntity(RefreshEntity refreshEntity, DateTimeOffset now);
    
    string CreateNewAccessFromRefreshEntity(RefreshEntity refreshEntity, DateTimeOffset now);

    Task SaveDatabaseChangesAsync();
}