using Database.Entity;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Interface.Service;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class RedirectValidationService(
    IOptions<OAuthOptions> oAuthOptions) : IRedirectValidationService
{
    public async Task<Result> ValidateRedirect(LoginRedirectInformation redirectInfo, OAuthProvider provider)
    {
        return await Task.Run(() =>
        {
            if (provider == OAuthProvider.Development)
            {
                return new Result();
            }
            
            var redirectSubdomains = redirectInfo.RedirectUri.Host.Split('.');
            var redirectTopDomain = $"{redirectSubdomains[^2]}.{redirectSubdomains[^1]}".ToLowerInvariant();
            
            var allowedSubdomains = oAuthOptions.Value.AllowedRedirectDomain.Split('.');
            var allowedTopDomain = $"{allowedSubdomains[^2]}.{allowedSubdomains[^1]}".ToLowerInvariant();

            if (redirectTopDomain != allowedTopDomain)
            {
                return new ResultError(
                    ResultErrorType.MapError, 
                    $"Invalid redirect top-domain \"{redirectTopDomain}\"");
            }
            
            return new Result();
        });
    }
}