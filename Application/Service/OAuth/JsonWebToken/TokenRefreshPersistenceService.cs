using Application.Service.OAuth.Login;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.JsonWebToken;

namespace Application.Service.OAuth.JsonWebToken;

public class TokenRefreshPersistenceService(
    TimeProvider timeProvider,
    IOptionsSnapshot<AuthOptions> authOptions,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext) : ITokenRefreshPersistenceService
{
    public Task<RefreshEntity?> GetRefreshEntity(
        RefreshEntityId refreshEntityId)
    {
        return databaseContext
            .Refresh
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == refreshEntityId);
    }

    public (RefreshEntity Entity, string Jwt) CreateRefreshEntity(
        RefreshEntity refreshEntity,
        DateTimeOffset now)
    {
        var tokenConfiguration = authOptions.Value.RefreshToken;
        var user = refreshEntity.User ?? throw new OAuthException(
            $"Unable to get user when creating token from {nameof(RefreshEntity)}");

        var tokenId = new RefreshEntityId(Guid.CreateVersion7());
        var jwtData = new JwtCreator.JwtData(
            Sub: user.Id.ToString(),
            Name: user.Username,
            Email: user.Email,
            Jti: tokenId.ToString(),
            Roles: user.Roles,
            Profile: user.RawAvatarUrl,
            AuthenticationProvider: user.AuthenticationProvider,
            AuthenticationProviderId: user.AuthenticationProviderId);
        var refreshToken = JwtCreator.CreateJwt(tokenConfiguration, jwtData, now);
        
        var connectionMetadata = httpContextAccessor.GetConnectionMetadata(timeProvider);
        var newRefreshEntity = new RefreshEntity
        {
            Id = tokenId,
            HashedRefreshToken = RefreshTokenHasher.Hash(refreshToken),
            LoginId = refreshEntity.LoginId,
            UserId = refreshEntity.UserId,
            User = refreshEntity.User,
            RemoteIpAddress = connectionMetadata.RemoteIpAddress,
            RemotePort = connectionMetadata.RemotePort,
            UserAgent = connectionMetadata.UserAgent,
            CreatedAt = connectionMetadata.CreatedAt,
            ValidUntil = timeProvider.GetUtcNow().UtcDateTime.Add(tokenConfiguration.Lifetime),
        };
        
        databaseContext.Refresh.Add(newRefreshEntity);
        
        return (Entity: newRefreshEntity, Jwt: refreshToken);
    }

    public AccessEntity CreateAccessEntityFromRefreshEntity(
        RefreshEntity refreshEntity,
        DateTimeOffset now)
    {
        var tokenConfiguration = authOptions.Value.AccessToken;
        var user = refreshEntity.User ?? throw new OAuthException(
            $"Unable to get user when creating token from {nameof(RefreshEntity)}");

        var tokenId = new AccessEntityId(Guid.CreateVersion7());
        var jwtData = new JwtCreator.JwtData(
            Sub: user.Id.ToString(),
            Name: user.Username,
            Email: user.Email,
            Jti: tokenId.ToString(),
            Roles: user.Roles,
            Profile: user.RawAvatarUrl,
            AuthenticationProvider: user.AuthenticationProvider,
            AuthenticationProviderId: user.AuthenticationProviderId);
        var accessToken = JwtCreator.CreateJwt(tokenConfiguration, jwtData, now);
        
        var connectionMetadata = httpContextAccessor.GetConnectionMetadata(timeProvider);
        var newAccessEntity = new AccessEntity
        {
            Id = tokenId,
            AccessToken = accessToken,
            RefreshId = refreshEntity.Id,
            Refresh = refreshEntity,
            LoginId = refreshEntity.LoginId,
            UserId = refreshEntity.UserId,
            User = refreshEntity.User,
            RemoteIpAddress = connectionMetadata.RemoteIpAddress,
            RemotePort = connectionMetadata.RemotePort,
            UserAgent = connectionMetadata.UserAgent,
            CreatedAt = connectionMetadata.CreatedAt,
            ValidUntil = timeProvider.GetUtcNow().UtcDateTime.Add(tokenConfiguration.Lifetime),
        };
        
        refreshEntity.Access.Add(newAccessEntity);
        return newAccessEntity;
    }

    public Task SaveDatabaseChangesAsync() => databaseContext.SaveChangesAsync();
}