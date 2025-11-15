using Database;
using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Login.Receive;

namespace Application.Service.OAuth.Login.Receive;

public class LoginReceiverRedirectService(
    ILogger<LoginReceiverRedirectService> logger,
    ITokenPersistenceService tokenPersistenceService,
    ILoginReceiverFactory receiverFactory,
    ILoginEntityFactory loginEntityFactory,
    ICookieApplier cookieApplier,
    TimeProvider timeProvider,
    DatabaseContext databaseContext) : ILoginReceiverRedirectService
{
    public async Task<Uri> PerformLoginAndCreateRedirectUri(
        AuthenticationProvider provider,
        Dictionary<string, string> parameters)
    {
        OAuthProcessEntity? oAuthProcessEntity = null;
        
        try
        {
            oAuthProcessEntity = await GetAndValidateOAuthProcess(provider, parameters);
            var loginReceiver = receiverFactory.Create(provider);
            var userConvertible = await loginReceiver.GetAuthUser(parameters);
            oAuthProcessEntity.FullUserJson = userConvertible.UserJson; // Update oAuthProcess with metadata
            
            var authenticatedUser = userConvertible.GetAuthenticatedUser();
            var login = await loginEntityFactory.CreateLogin(
                authenticatedUser,
                oAuthProcessEntity);
            
            var tokenPair = await tokenPersistenceService.CreateAndPersistTokenPair(
                login,
                oAuthProcessEntity);
            
            cookieApplier.SetCookie(TokenType.Refresh, tokenPair.RefreshToken.Token);
            cookieApplier.SetCookie(TokenType.Access, tokenPair.AccessToken.Token);
            return oAuthProcessEntity.SuccessRedirectUri;
        }
        catch (Exception e)
        {
            if (oAuthProcessEntity is null)
            {
                throw;
            }
            
            logger.LogWarning(
                e,
                "Error in login receiver redirect for {ProcessName} with state {State}",
                nameof(oAuthProcessEntity),
                oAuthProcessEntity.State);
            
            oAuthProcessEntity.Error = e.Message;
            await databaseContext.SaveChangesAsync();
            return new OAuthUriBuilder(oAuthProcessEntity.ErrorRedirectUri)
                .AddQueryParam("error", Uri.EscapeDataString(oAuthProcessEntity.Error))
                .Build();;
        }
    }

    private async Task<OAuthProcessEntity> GetAndValidateOAuthProcess(
        AuthenticationProvider provider,
        Dictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue("state", out var state))
        {
            throw new OAuthException("A user hit the login-receiver endpoint and no state was found");
        }
        
        var oAuthProcessEntity = await databaseContext
            .OAuthProcess
            .FirstOrDefaultAsync(p => p.State == state);

        if (oAuthProcessEntity is null)
        {
            throw new OAuthException($"A user hit the login-receiver endpoint and no {nameof(OAuthProcessEntity)} was found for state '{state}'");
        }
            
        if (!Enum.TryParse<AuthenticationProvider>(oAuthProcessEntity.AuthenticationProvider, ignoreCase: true, out var oAuthProcessAuthenticationProvider))
        {
            throw new OAuthException($"A user hit the login-receiver endpoint with state '{state}' and the {nameof(OAuthProcessEntity)}.{nameof(OAuthProcessEntity.AuthenticationProvider)} cannot be parsed.");
        }

        if (oAuthProcessAuthenticationProvider != provider)
        {
            throw new OAuthException($"A user hit the login-receiver endpoint and the {nameof(OAuthProcessEntity)} for '{state}' does not match the process {nameof(AuthenticationProvider)} value.");
        }

        return timeProvider.GetUtcNow().UtcDateTime - oAuthProcessEntity.CreatedAt > TimeSpan.FromHours(1)
            ? throw new OAuthException($"A user hit the login-receiver endpoint with state '{state}' and the {nameof(OAuthProcessEntity)}.{nameof(OAuthProcessEntity.CreatedAt)} is older than one hour making it invalid - try again.")
            : oAuthProcessEntity;
    }
}