using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Presentation.Service.OAuth.Login.Receive;
using Presentation.Service.OAuth.Model;

namespace Application.Service.OAuth.Login.Receive;

// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage
public class LoginEntityFactory(
    TimeProvider timeProvider,
    DatabaseContext databaseContext) : ILoginEntityFactory
{
    public async Task<LoginEntity> CreateLogin(
        AuthenticatedUser user,
        OAuthProcessEntity process,
        Action<UserEntity> onNewUser)
    {
        var firstLogin = false;
        var userEntity = await databaseContext
            .User
            .FirstOrDefaultAsync(u => u.AuthenticationProviderId == user.AuthenticationProviderId);

        if (userEntity is null)
        {
            userEntity = new UserEntity
            {
                Id = new UserEntityId(Guid.CreateVersion7()),
                Username = user.Username,
                AuthenticationProviderId = user.AuthenticationProviderId,
                AuthenticationProvider = process.AuthenticationProvider,
                Email =  user.Email,
                RawAvatarUrl = user.AvatarUrl,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            };
            
            firstLogin = true;
        }
        
        var loginEntity = new LoginEntity
        {
            Id = new LoginEntityId(Guid.CreateVersion7()),
            UserId = userEntity.Id,
            User = userEntity,
            OAuthProcessId =  process.Id,
            OAuthProcess = process,
            FirstLogin = firstLogin,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
        
        userEntity.Login.Add(loginEntity);

        if (firstLogin)
        {
            onNewUser(userEntity);
        }

        return loginEntity;
    }
}