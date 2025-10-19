using Database.Entity;

namespace Presentation.Service.OAuth;

public interface IOAuthProcessFactory
{
    OAuthProcessEntity CreateProcess(AuthenticationProvider authenticationProvider);
}