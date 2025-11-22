using System.ComponentModel.DataAnnotations;

namespace Presentation.Configuration.Options;

public class DiscordWebhookOptions : IConfigurationOptions
{
    public static string SectionName => "DiscordWebhook";

    [Required]
    [Url]
    public required Uri Url { get; init; }
}
