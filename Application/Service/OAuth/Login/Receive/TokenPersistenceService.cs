using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Login.Receive;
using Presentation.Service.OAuth.Model.Token;

namespace Application.Service.OAuth.Login.Receive;

/// <inheritdoc />
public class TokenPersistenceService(
    IOptionsSnapshot<AuthOptions> authOptions,
    DatabaseContext databaseContext,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    IJsonWebTokenFactory jsonWebTokenFactory) : ITokenPersistenceService
{
    public async Task<TokenPair> CreateAndPersistTokenPair(
        LoginEntity loginEntity,
        OAuthProcessEntity oAuthProcessEntity)
    {
        var connectionMetadata = httpContextAccessor.GetConnectionMetadata(timeProvider);
        var refreshId = new RefreshEntityId(Guid.CreateVersion7());
        var accessId = new AccessEntityId(Guid.CreateVersion7());
        var tokenPair = jsonWebTokenFactory.CreateTokenPairFromNewLoginEntity(
            loginEntity,
            refreshId,
            accessId);

        var refreshEntity = this.CreateRefreshEntity(
            refreshId,
            tokenPair.RefreshToken,
            loginEntity,
            connectionMetadata);

        var accessEntity = this.CreateAccessEntityFromRefreshEntity(
            accessId,
            refreshEntity,
            tokenPair.AccessToken,
            loginEntity,
            connectionMetadata);
        
        UpdateOAuthProcessEntity(oAuthProcessEntity, loginEntity);
        
        refreshEntity.Access.Add(accessEntity);
        loginEntity.Refresh.Add(refreshEntity);
        databaseContext.Login.Add(loginEntity);
        await databaseContext.SaveChangesAsync();

        return tokenPair;
    }

    private RefreshEntity CreateRefreshEntity(
        RefreshEntityId refreshEntityId,
        RefreshToken refreshToken,
        LoginEntity loginEntity,
        BaseConnectionMetadata connectionMetadata)
    {
        var refreshLifetime = authOptions.Value.RefreshToken.Lifetime;
        return new RefreshEntity
        {
            Id = refreshEntityId,
            HashedRefreshToken = RefreshTokenHasher.Hash(refreshToken.Token),
            LoginId = loginEntity.Id,
            Login = loginEntity,
            UserId = loginEntity.UserId,
            User = loginEntity.User,
            RemoteIpAddress = connectionMetadata.RemoteIpAddress,
            RemotePort = connectionMetadata.RemotePort,
            UserAgent = connectionMetadata.UserAgent,
            CreatedAt = connectionMetadata.CreatedAt,
            ValidUntil = timeProvider.GetUtcNow().UtcDateTime.Add(refreshLifetime),
        };
    }
    
    private AccessEntity CreateAccessEntityFromRefreshEntity(
        AccessEntityId accessEntityId,
        RefreshEntity refreshEntity,
        AccessToken accessToken,
        LoginEntity loginEntity,
        BaseConnectionMetadata connectionMetadata)
    {
        var accessLifetime = authOptions.Value.AccessToken.Lifetime;
        return new AccessEntity
        {
            Id = accessEntityId,
            AccessToken = accessToken.Token,
            RefreshId = refreshEntity.Id,
            Refresh = refreshEntity,
            LoginId = loginEntity.Id,
            Login = loginEntity,
            UserId = loginEntity.UserId,
            User = loginEntity.User,
            RemoteIpAddress = connectionMetadata.RemoteIpAddress,
            RemotePort = connectionMetadata.RemotePort,
            UserAgent = connectionMetadata.UserAgent,
            CreatedAt = connectionMetadata.CreatedAt,
            ValidUntil = timeProvider.GetUtcNow().UtcDateTime.Add(accessLifetime),
        };
    }

    private static void UpdateOAuthProcessEntity(
        OAuthProcessEntity oAuthProcessEntity,
        LoginEntity loginEntity)
    {
        oAuthProcessEntity.UserId = loginEntity.UserId;
        oAuthProcessEntity.User = loginEntity.User;
        oAuthProcessEntity.LoginId = loginEntity.Id;
        oAuthProcessEntity.Login = loginEntity;
    }
}