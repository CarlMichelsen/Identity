using System.Threading.Channels;
using Application.Configuration;
using Database;
using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Presentation;
using Presentation.Client.Discord;
using Presentation.Service.OAuth;
using Presentation.Service.OAuth.Login.Receive;

namespace Application.Service.OAuth.Login.Receive;

public class LoginReceiverRedirectService(
    ILoginReceiverFactory receiverFactory,
    ILoginEntityFactory loginEntityFactory,
    TimeProvider timeProvider,
    DatabaseContext databaseContext,
    Channel<WebhookMessage> channel) : ILoginReceiverRedirectService
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
            var authenticatedUser = userConvertible.GetAuthenticatedUser();

            var login = await loginEntityFactory.CreateLogin(
                authenticatedUser,
                oAuthProcessEntity,
                NotifyOnNewSignup);
            databaseContext.Login.Add(login);
            
            // TODO: Create Access and Refresh tokens
            
            await databaseContext.SaveChangesAsync();
            return oAuthProcessEntity.SuccessRedirectUri;
        }
        catch (Exception e)
        {
            if (oAuthProcessEntity is null)
            {
                throw;
            }
            
            oAuthProcessEntity.Error = e.Message;
            await databaseContext.SaveChangesAsync();
            return new OAuthUriBuilder(oAuthProcessEntity.ErrorRedirectUri)
                .AddQueryParam("error", oAuthProcessEntity.Error)
                .Build();;
        }
    }

    private void NotifyOnNewSignup(UserEntity user)
    {
        channel.Writer.TryWrite(new WebhookMessage(
            Username: $"{ApplicationConstants.Name} - {ApplicationConstants.Version}",
            Content: $"New user '{user.Username}' signed up",
            Embeds:
            [
                new WebhookEmbed
                {
                    Title = "Id",
                    Description = user.Id.Value.ToString(),
                },
                new WebhookEmbed
                {
                    Title = "Email",
                    Url = user.Email
                },
                new WebhookEmbed
                {
                    Title = "Signed up",
                    Description = user.CreatedAt.ToString("o")
                }
            ]));
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

        if (timeProvider.GetUtcNow().UtcDateTime - oAuthProcessEntity.CreatedAt > TimeSpan.FromHours(1))
        {
            throw new OAuthException($"A user hit the login-receiver endpoint with state '{state}' and the {nameof(OAuthProcessEntity)}.{nameof(OAuthProcessEntity.CreatedAt)} is older than one hour making it invalid - try again.");
        }
            
        return oAuthProcessEntity;

    }
}