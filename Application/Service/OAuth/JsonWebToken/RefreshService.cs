using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Service.OAuth.Login;
using Database.Entity.Id;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Presentation;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth.JsonWebToken;

namespace Application.Service.OAuth.JsonWebToken;

public class RefreshService(
    ILogger<RefreshService> logger,
    TimeProvider timeProvider,
    IOptionsSnapshot<AuthOptions> authOptions,
    ITokenRefreshPersistenceService  tokenRefreshPersistenceService,
    ICookieApplier cookieApplier,
    ITokenValidator tokenValidator) : IRefreshService
{
    public async Task<bool> HandleRefresh()
    {
        var now = timeProvider.GetUtcNow();
        var refreshToken = tokenValidator.Validate(TokenType.Refresh, validateLifetime: true);
        var accessToken = tokenValidator.Validate(TokenType.Access, validateLifetime: false);
        if (refreshToken is null)
        {
            return false;
        }
        
        var refreshEntity = await tokenRefreshPersistenceService
            .GetRefreshEntity(GetRefreshEntityId(refreshToken.Value.ClaimsPrincipal));
        if (refreshEntity is null)
        {
            // Unable to find refresh-token in the database
            return false;
        }
        
        // This is a double verification because the tokenValidator does almost the exact same thing - keeping this as a sanity check.
        if (!RefreshTokenHasher.Verify(refreshToken.Value.JwtValue, refreshEntity.HashedRefreshToken))
        {
            logger.LogCritical(
                "This refresh token <{RefreshEntityId}> was created by this login service but differs from the version in the database (extremely scary)",
                refreshEntity.Id);
            return false;
        }

        // If the refresh token is invalidated - the user is not supposed to have access.
        if (refreshEntity.ValidUntil <= now)
        {
            logger.LogInformation(
                "A user <{UserId}> had their refresh-token invalidated.",
                refreshEntity.UserId);
            
            cookieApplier.DeleteCookie(TokenType.Refresh);
            cookieApplier.DeleteCookie(TokenType.Access);
            return false;
        }
        
        string? refreshJsonWebToken = null;
        var refreshTokenShouldBeRefreshed =
            TokenShouldBeRefreshed(TokenType.Refresh, refreshToken.Value.ClaimsPrincipal, now);
        if (refreshTokenShouldBeRefreshed)
        {
            refreshEntity.ValidUntil = now.UtcDateTime;
            var tokenData = tokenRefreshPersistenceService.CreateRefreshEntity(refreshEntity, now);
            refreshEntity = tokenData.Entity;
            refreshJsonWebToken = tokenData.Jwt;
        }
        
        string? accessJsonWebToken = null;
        var accessTokenShouldBeRefreshed =
            TokenShouldBeRefreshed(TokenType.Access, accessToken?.ClaimsPrincipal, now);
        if (accessTokenShouldBeRefreshed || refreshTokenShouldBeRefreshed)
        {
            accessJsonWebToken = tokenRefreshPersistenceService.CreateNewAccessFromRefreshEntity(refreshEntity, now);
        }

        await tokenRefreshPersistenceService.SaveDatabaseChangesAsync();
        
        if (refreshJsonWebToken is not null)
        {
            logger.LogARefreshTokenWasMintedByUserUseridWithIdRefreshEntityId(refreshEntity.UserId, refreshEntity.Id);
            cookieApplier.SetCookie(TokenType.Refresh, refreshJsonWebToken);
        }

        if (accessJsonWebToken is not null)
        {
            logger.LogAnAccessTokenWasMintedByUser(refreshEntity!.UserId);
            cookieApplier.SetCookie(TokenType.Access, accessJsonWebToken);
        }

        return true;
    }

    private static DateTimeOffset GetExpirationTime(ClaimsPrincipal principal, DateTimeOffset now)
    {
        var accessExpirationClaim = principal
            .Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
        
        DateTimeOffset? accessExpiration = long.TryParse(accessExpirationClaim?.Value, out var epochExpirationTime)
            ? DateTimeOffset.FromUnixTimeSeconds(epochExpirationTime)
            : null;

        return accessExpiration ?? now;
    }

    private static RefreshEntityId GetRefreshEntityId(ClaimsPrincipal principal)
    {
        var accessJtiClaim = principal
            .Claims
            .First(c => c.Type == JwtTokenKeys.Jti);

        return new RefreshEntityId(Guid.Parse(accessJtiClaim.Value), true);
    }

    private bool TokenShouldBeRefreshed(TokenType tokenType, ClaimsPrincipal? principal, DateTimeOffset now)
    {
        if (principal is null)
        {
            return true;
        }
        
        var accessExpirationTime = GetExpirationTime(principal, now);
        var timeToAccessExpiration = accessExpirationTime - now;

        var tokenConfig = tokenType switch
        {
            TokenType.Access => authOptions.Value.AccessToken,
            TokenType.Refresh => authOptions.Value.RefreshToken,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };

        // If the minimum lifetime of the token has passed then create a new token - otherwise do nothing.
        var accessExpirationThreshold = tokenConfig.Lifetime - tokenConfig.MinimumLifetime;
        return timeToAccessExpiration <= accessExpirationThreshold;
    }
}