namespace Domain.OAuth;

public class LoginProcessContext
{
    public required LoginProcessIdentifier Identifier { get; init; }
    
    public required LoginRedirectInformation Redirect { get; init; }
    
    public IUserConvertible? User { get; set; }
}