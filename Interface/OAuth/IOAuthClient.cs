using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.OAuth;

public interface IOAuthClient
{
    Task<Result<Uri>> CreateOAuthRedirect(LoginRedirectInformation loginRedirectInformation);
    
    Task<Result<LoginProcessContext>> CompleteLogin(Dictionary<string, string> queryParameters);
}