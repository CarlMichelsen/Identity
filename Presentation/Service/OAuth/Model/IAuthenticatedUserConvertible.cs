namespace Presentation.Service.OAuth.Model;

public interface IAuthenticatedUserConvertible
{
    AuthenticatedUser GetAuthenticatedUser();
    
    string UserJson { get; }
}