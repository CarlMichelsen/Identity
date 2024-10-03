using Domain.OAuth;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IOAuthRedirectHandler
{
    Task<IResult> CreateOAuthRedirect(
        string oAuthProviderString,
        string postLoginRedirectUrl);
}