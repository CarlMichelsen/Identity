using Domain.Abstraction;

namespace Domain.OAuth.Development;

public class DevelopmentUserDto : IOAuthUserConvertible
{
    public required string Id { get; init; }
    
    public required string Username { get; init; }
    
    public required string AvatarUrl { get; init; }
    
    public required string Email { get; init; }
    
    public Result<OAuthUser> ToUser()
    {
        return OAuthValidator.Validate(
            id: this.Id,
            provider: OAuthProvider.Development,
            username: this.Username,
            email: this.Email,
            avatarUrl: this.AvatarUrl);
    }
}