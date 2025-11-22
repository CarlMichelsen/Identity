using Database.Entity;
using Database.Entity.Id;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Model.Token;

namespace Application.Service.OAuth.JsonWebToken;

public class JsonWebTokenFactory(
    TimeProvider timeProvider,
    IOptionsSnapshot<AuthOptions> authOptions,
    IUserImageUriFactory userImageUriFactory) : IJsonWebTokenFactory
{
    public TokenPair CreateTokenPairFromNewLoginEntity(
        LoginEntity loginEntity,
        RefreshEntityId refreshEntityId)
    {
        return new TokenPair
        {
            RefreshToken = new RefreshToken { Token = CreateJwtToken(TokenType.Refresh, loginEntity, refreshEntityId.Value.ToString()) },
            AccessToken = new AccessToken { Token = CreateJwtToken(TokenType.Access, loginEntity, Guid.CreateVersion7().ToString()) },
        };
    }

    private string CreateJwtToken(TokenType tokenType, LoginEntity loginEntity, string tokenId)
    {
        var now = timeProvider
            .GetUtcNow();
        
        var tokenConfiguration = tokenType switch
        {
            TokenType.Access => authOptions.Value.AccessToken,
            TokenType.Refresh => authOptions.Value.RefreshToken,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };

        var user = loginEntity.User ?? throw new OAuthException(
            $"Unable to get user when creating token from {nameof(LoginEntity)}");
        
        var jwtData = new JwtCreator.JwtData(
            Sub: user.Id.ToString(),
            Name: user.Username,
            Email: user.Email,
            Jti: tokenId,
            Roles: user.Roles,
            Small: user.Image is null ? user.RawAvatarUrl : userImageUriFactory.GetSmallImageUri(user.Image),
            Medium: user.Image is null ? null :  userImageUriFactory.GetMediumImageUri(user.Image),
            Large: user.Image is null ? null :userImageUriFactory.GetLargeImageUri(user.Image) ,
            AuthenticationProvider: user.AuthenticationProvider,
            AuthenticationProviderId: user.AuthenticationProviderId);

        return JwtCreator.CreateJwt(tokenConfiguration, jwtData, now);
    }
}