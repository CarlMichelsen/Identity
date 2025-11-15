using System.Threading.Channels;
using Application.Configuration;
using Database.Entity;
using Presentation.Client.Discord;
using Presentation.Service.OAuth.Login.Receive;

namespace Application.Service.OAuth.Login.Receive;

public class FirstLoginNotifier(
    TimeProvider timeProvider,
    Channel<WebhookMessage> channel) : IFirstLoginNotifier
{
    public void NotifyFirstLogin(UserEntity userEntity)
    {
        channel.Writer.TryWrite(new WebhookMessage(
            Username: $"{ApplicationConstants.Name} - {ApplicationConstants.Version}",
            Content: $"New user **{userEntity.Id.Value}** signed up",
            Embeds:
            [
                new WebhookEmbed
                {
                    Title = userEntity.Username,
                    Description = userEntity.Email,
                    Timestamp = timeProvider.GetUtcNow().UtcDateTime,
                    Url = userEntity.RawAvatarUrl.AbsoluteUri,
                },
            ]));
    }
}