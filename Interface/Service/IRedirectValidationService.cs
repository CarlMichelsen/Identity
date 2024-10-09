using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Service;

public interface IRedirectValidationService
{
    Task<Result> ValidateRedirect(LoginRedirectInformation redirectInfo, OAuthProvider provider);
}