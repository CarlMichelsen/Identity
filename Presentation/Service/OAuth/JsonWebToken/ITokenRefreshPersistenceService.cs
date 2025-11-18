using Database.Entity;
using Database.Entity.Id;

namespace Presentation.Service.OAuth.JsonWebToken;

public interface ITokenRefreshPersistenceService
{
    Task<RefreshEntity?> GetRefreshEntity(RefreshEntityId refreshEntityId);
    
    (RefreshEntity Entity, string Jwt) CreateRefreshEntity(RefreshEntity refreshEntity, DateTimeOffset now);
    
    AccessEntity CreateAccessEntityFromRefreshEntity(RefreshEntity refreshEntity, DateTimeOffset now);

    Task SaveDatabaseChangesAsync();
}