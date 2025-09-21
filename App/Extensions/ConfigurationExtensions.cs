using Interface.Configuration;

namespace App.Extensions;

public static class ConfigurationExtensions
{
    public static IHostApplicationBuilder AddConfigurationSection<TSection>(
        this IHostApplicationBuilder builder)
        where TSection : class, IOptionsSection
    {
        builder
            .Services
            .Configure<TSection>(
                builder.Configuration.GetSection(TSection.SectionName));
        return builder;
    }
}
