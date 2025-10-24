namespace Presentation.Configuration.Options.Provider;

public class DiscordProvider : BaseProvider
{
    public override AuthenticationProvider ProviderType { get; } = AuthenticationProvider.Discord;
}