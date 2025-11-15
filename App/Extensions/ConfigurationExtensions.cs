using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;

namespace App.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers a configuration options class that implements <see cref="IConfigurationOptions"/>.
    /// Binds it to the appropriate section, enables data annotation validation,
    /// and validates it on startup.
    /// Does not register *IOptions* in services.
    /// Use *IOptionsSnapshot* or if change during scope is ok, or you're working in a singleton then use *IOptionsMonitor*.
    /// </summary>
    public static IServiceCollection AddConfigurationOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TOptions : class, IConfigurationOptions
    {
        ArgumentNullException.ThrowIfNull(
            configuration,
            $"Unable to get IConfiguration when registering {TOptions.SectionName}");

        var section = configuration.GetSection(TOptions.SectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section '{TOptions.SectionName}' is missing for options type '{typeof(TOptions).Name}'.");
        }

        services
            .AddOptions<TOptions>()
            .Bind(section, options => options.ErrorOnUnknownConfiguration = true)
            .ValidateDataAnnotations() // runs [Required], [Range], etc.
            .ValidateOnStart();
        
        // IOptions can change during the scope execution and that makes me uneasy - removing it from DI
        services.RemoveAll<IOptions<TOptions>>();

        return services;
    }
}
