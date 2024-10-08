using Domain.Dto;

namespace Domain.OAuth;

public class LoginProcessContext
{
    public required LoginProcessIdentifier Identifier { get; init; }
    
    public required LoginRedirectInformation Redirect { get; init; }
    
    public IOAuthUserConvertible? OAuthUserConvertible { get; set; }
    
    public AuthenticatedUser? User { get; set; }
    
    public long? LoginId { get; set; }
    
    public long? RefreshId { get; set; }
    
    public long? AccessId { get; set; }
    
    public string? RefreshJwtId { get; set; }
    
    public string? AccessJwtId { get; set; }
}