using Application.Service.OAuth.Login;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Refresh;

namespace Application.Service.OAuth.Refresh;

public class TokenRefreshPersistenceService(
    TimeProvider timeProvider,
    IOptionsSnapshot<AuthOptions> authOptions,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext,
    IHostEnvironment hostEnvironment,
    IUserImageUriFactory userImageUriFactory) : ITokenRefreshPersistenceService
{
    public Task<RefreshEntity?> GetRefreshEntity(
        RefreshEntityId refreshEntityId)
    {
        return databaseContext
            .Refresh
            .Include(r => r.User)
            .ThenInclude(u => u!.Image)
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
            Small: user.Image is null ? user.RawAvatarUrl : userImageUriFactory.GetSmallImageUri(user.Image),
            Medium: user.Image is null ? null : userImageUriFactory.GetMediumImageUri(user.Image),
            Large: user.Image is null ? null : userImageUriFactory.GetLargeImageUri(user.Image),
            AuthenticationProvider: user.AuthenticationProvider,
            AuthenticationProviderId: user.AuthenticationProviderId);
        var refreshToken = JwtCreator.CreateJwt(tokenConfiguration, jwtData, now);
        
        var connectionMetadata = httpContextAccessor.GetConnectionMetadata(timeProvider, hostEnvironment);
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

    public string CreateNewAccessFromRefreshEntity(
        RefreshEntity refreshEntity,
        DateTimeOffset now)
    {
        var tokenConfiguration = authOptions.Value.AccessToken;
        var user = refreshEntity.User ?? throw new OAuthException(
            $"Unable to get user when creating token from {nameof(RefreshEntity)}");

        var jwtData = new JwtCreator.JwtData(
            Sub: user.Id.ToString(),
            Name: user.Username,
            Email: user.Email,
            Jti: Guid.CreateVersion7().ToString(),
            Roles: user.Roles,
            Small: user.Image is null ? user.RawAvatarUrl : userImageUriFactory.GetSmallImageUri(user.Image),
            Medium: user.Image is null ? null : userImageUriFactory.GetMediumImageUri(user.Image),
            Large: user.Image is null ? null : userImageUriFactory.GetLargeImageUri(user.Image),
            AuthenticationProvider: user.AuthenticationProvider,
            AuthenticationProviderId: user.AuthenticationProviderId);
        return JwtCreator.CreateJwt(tokenConfiguration, jwtData, now);
    }

    public Task SaveDatabaseChangesAsync() => databaseContext.SaveChangesAsync();
}