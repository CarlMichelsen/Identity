using Interface.Configuration;

namespace Implementation.Configuration.Options;

public class DiscordWebhookOptions : IOptionsSection
{
    public static string SectionName => "DiscordWebhook";

    public required Uri Url { get; init; }
}
