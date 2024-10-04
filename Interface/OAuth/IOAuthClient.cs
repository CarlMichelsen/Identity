using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.OAuth;

public interface IOAuthClient
{
    Task<Result<Uri>> CreateOAuthRedirect(LoginRedirectInformation loginRedirectInformation);
    
    Task<Result<LoginProcessContext>> GetProviderLoginData(Dictionary<string, string> queryParameters);
}