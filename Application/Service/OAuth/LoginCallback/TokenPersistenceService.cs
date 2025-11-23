using Application.Service.OAuth.Login;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service.OAuth.Refresh;
using Presentation.Service.OAuth.LoginCallback;
using Presentation.Service.OAuth.Model.Token;

namespace Application.Service.OAuth.LoginCallback;

/// <inheritdoc />
public class TokenPersistenceService(
    IOptionsSnapshot<AuthOptions> authOptions,
    DatabaseContext databaseContext,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    IHostEnvironment hostEnvironment,
    IJsonWebTokenFactory jsonWebTokenFactory) : ITokenPersistenceService
{
    public async Task<TokenPair> CreateAndPersistTokenPair(
        LoginEntity loginEntity,
        OAuthProcessEntity oAuthProcessEntity)
    {
        var connectionMetadata = httpContextAccessor.GetConnectionMetadata(timeProvider, hostEnvironment);
        var refreshId = new RefreshEntityId(Guid.CreateVersion7());
        var tokenPair = jsonWebTokenFactory.CreateTokenPairFromNewLoginEntity(
            loginEntity,
            refreshId);

        var refreshEntity = this.CreateRefreshEntity(
            refreshId,
            tokenPair.RefreshToken,
            loginEntity,
            connectionMetadata);
        
        UpdateOAuthProcessEntity(oAuthProcessEntity, loginEntity);
        
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