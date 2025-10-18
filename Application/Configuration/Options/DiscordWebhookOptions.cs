using System.ComponentModel.DataAnnotations;
using Presentation.Configuration;

namespace Application.Configuration.Options;

public class DiscordWebhookOptions : IConfigurationOptions
{
    public static string SectionName => "DiscordWebhook";

    [Required]
    [Url]
    public required Uri Url { get; init; }
}
