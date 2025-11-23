using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Presentation.Service.OAuth.LoginCallback;
using Presentation.Service.OAuth.Model;

namespace Application.Service.OAuth.LoginCallback;

// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage
public class LoginEntityFactory(
    TimeProvider timeProvider,
    DatabaseContext databaseContext,
    IFirstLoginNotifier firstLoginNotifier) : ILoginEntityFactory
{
    public async Task<LoginEntity> CreateLogin(
        AuthenticatedUser user,
        OAuthProcessEntity process)
    {
        var firstLogin = false;
        var userEntity = await databaseContext
            .User
            .Include(u => u.Image)
            .FirstOrDefaultAsync(u => u.AuthenticationProviderId == user.AuthenticationProviderId);

        var now = timeProvider.GetUtcNow().UtcDateTime;
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
                CreatedAt = now,
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
            CreatedAt = now,
        };
        
        userEntity.Login.Add(loginEntity);

        if (firstLogin)
        {
            firstLoginNotifier.NotifyFirstLogin(userEntity);
        }

        return loginEntity;
    }
}