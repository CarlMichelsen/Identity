using Domain.Dto;

namespace Domain.OAuth;

public class LoginProcessContext
{
    public required LoginProcessIdentifier Identifier { get; init; }
    
    public required LoginRedirectInformation Redirect { get; init; }
    
    public IOAuthUserConvertible? OAuthUserConvertible { get; set; }
    
    public UserDto? User { get; set; }
    
    public long? LoginId { get; set; }
    
    public long? RefreshId { get; set; }
}