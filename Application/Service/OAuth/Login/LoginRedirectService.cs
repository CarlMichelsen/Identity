using Database;
using Microsoft.Extensions.Logging;
using Presentation;
using Presentation.Dto;
using Presentation.Service.OAuth.Login;

namespace Application.Service.OAuth.Login;

public class LoginRedirectService(
    ILogger<LoginRedirectService> logger,
    IOAuthProcessEntityFactory ioAuthProcessEntityFactory,
    DatabaseContext databaseContext) : ILoginRedirectService
{
    public async Task<Uri> GetLoginRedirectUri(AuthenticationProvider provider, LoginQueryDto loginQueryDto)
    {
        try
        {
            var oAuthProcessEntity = ioAuthProcessEntityFactory.CreateProcess(
                authenticationProvider: provider,
                successRedirectUrl: loginQueryDto.SuccessRedirectUrl,
                errorRedirectUrl: loginQueryDto.ErrorRedirectUrl);
            
            databaseContext.OAuthProcess.Add(oAuthProcessEntity);
            await databaseContext.SaveChangesAsync();

            return oAuthProcessEntity.LoginRedirectUri;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get login redirect uri");
            return loginQueryDto.ErrorRedirectUrl;
        }
    }
}