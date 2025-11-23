using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Database;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Refresh;
using Presentation.Service.OAuth.Login;

namespace Application.Service.OAuth.Login;

public class LogoutService(
    ILogger<LogoutService> logger,
    TimeProvider timeProvider,
    DatabaseContext dbContext,
    ICookieApplier cookieApplier,
    ITokenValidator tokenValidator) : ILogoutService
{
    public async Task<bool> HandleLogout()
    {
        var now = timeProvider
            .GetUtcNow()
            .UtcDateTime;
        var refreshToken = tokenValidator.Validate(TokenType.Refresh, validateLifetime: true);
        if (refreshToken is null)
        {
            return false;
        }

        var refreshId = GetRefreshEntityId(refreshToken.Value.ClaimsPrincipal);
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
        var refreshEntity = await dbContext
            .Refresh
            .FirstOrDefaultAsync(r => r.Id == refreshId);

        if (refreshEntity is null)
        {
            return false;
        }
        
        if (refreshEntity.ValidUntil < now)
        {
            return false;
        }
        
        refreshEntity.ValidUntil = now;
        
        await dbContext.SaveChangesAsync();
        cookieApplier.DeleteCookie(TokenType.Access);
        cookieApplier.DeleteCookie(TokenType.Refresh);
        logger.LogInformation(
            "User <{UserId}> logged out successfully",
            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
            refreshEntity.UserId);

        return true;
    }
    
    private static RefreshEntityId GetRefreshEntityId(ClaimsPrincipal principal)
    {
        var accessJtiClaim = principal
            .Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Jti);
        return new RefreshEntityId(Guid.Parse(accessJtiClaim.Value), true);
    }
}