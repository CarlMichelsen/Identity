using System.Threading.Channels;
using Presentation.Configuration;
using Database.Entity;
using Microsoft.Extensions.Hosting;
using Presentation.Client.Discord;
using Presentation.Service.OAuth.LoginCallback;

namespace Application.Service.OAuth.LoginCallback;

public class FirstLoginNotifier(
    TimeProvider timeProvider,
    IHostEnvironment environment,
    Channel<WebhookMessage> channel) : IFirstLoginNotifier
{
    public void NotifyFirstLogin(UserEntity userEntity)
    {
        channel.Writer.TryWrite(new WebhookMessage(
            Username: $"{environment.EnvironmentName} - {ApplicationConstants.Name} - {ApplicationConstants.Version}",
            Content: $"New user **{userEntity.Id.Value}** signed up using {userEntity.AuthenticationProvider}",
            Embeds:
            [
                new WebhookEmbed
                {
                    Title = userEntity.Username,
                    Description = userEntity.Email,
                    Timestamp = timeProvider.GetUtcNow().UtcDateTime,
                },
            ]));
    }
}