using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Extensions;

/// <summary>
/// Extension methods para ILoggingBuilder.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Define o nível mínimo de log para todos os providers.
    /// </summary>
    public static ILoggingBuilder SetMinimumLevel(this ILoggingBuilder builder, LogLevel minimumLevel)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.SetMinimumLevel(minimumLevel);
        return builder;
    }

    /// <summary>
    /// Limpa todos os providers de logging.
    /// </summary>
    public static ILoggingBuilder ClearAllProviders(this ILoggingBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.ClearProviders();
        return builder;
    }
}
