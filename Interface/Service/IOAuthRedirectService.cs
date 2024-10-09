using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Service;

public interface IOAuthRedirectService
{
    Task<Result<Uri>> CreateOAuthRedirect(
        OAuthProvider provider,
        LoginRedirectInformation loginRedirectInformation);
}