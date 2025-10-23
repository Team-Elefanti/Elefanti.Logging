using Elefanti.Logging.Discord.Client;
using Elefanti.Logging.Discord.Configuration;
using Elefanti.Logging.Discord.Formatting;
using Elefanti.Logging.Discord.Providers;
using Elefanti.Logging.Discord.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Elefanti.Logging.Discord.Extensions;

/// <summary>
/// Extension methods para configuração do Discord Logger.
/// </summary>
public static class DiscordLoggingExtensions
{
    /// <summary>
    /// Adiciona o Discord logger ao builder de logging.
    /// </summary>
    /// <param name="builder">O ILoggingBuilder.</param>
    /// <param name="configure">Action para configurar as opções.</param>
    /// <returns>O ILoggingBuilder para encadeamento.</returns>
    public static ILoggingBuilder AddDiscord(
        this ILoggingBuilder builder,
        Action<DiscordLoggerOptions> configure)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var options = new DiscordLoggerOptions();
        configure(options);

        // Valida as opções
        DiscordOptionsValidator.Validate(options);

        // Registra as dependências
        builder.Services.TryAddSingleton(options);
        builder.Services.TryAddSingleton<DiscordMessageFormatter>();
        
        // Registra DiscordWebhookClient com HttpClient dedicado (sem IHttpClientFactory para evitar circular dependency)
        builder.Services.TryAddSingleton<DiscordWebhookClient>(sp =>
        {
            // Cria HttpClient manualmente - não usa IHttpClientFactory para evitar dependência circular
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            };
            
            return new DiscordWebhookClient(httpClient, options);
        });

        // Registra o provider
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, DiscordLoggerProvider>());

        return builder;
    }

    /// <summary>
    /// Adiciona o Discord logger ao builder de logging com webhook URL.
    /// </summary>
    /// <param name="builder">O ILoggingBuilder.</param>
    /// <param name="webhookUrl">URL do webhook do Discord.</param>
    /// <returns>O ILoggingBuilder para encadeamento.</returns>
    public static ILoggingBuilder AddDiscord(
        this ILoggingBuilder builder,
        string webhookUrl)
    {
        return builder.AddDiscord(options =>
        {
            options.WebhookUrl = webhookUrl;
        });
    }

    /// <summary>
    /// Adiciona o Discord logger ao builder de logging com webhook URL e username.
    /// </summary>
    /// <param name="builder">O ILoggingBuilder.</param>
    /// <param name="webhookUrl">URL do webhook do Discord.</param>
    /// <param name="username">Nome de usuário personalizado.</param>
    /// <returns>O ILoggingBuilder para encadeamento.</returns>
    public static ILoggingBuilder AddDiscord(
        this ILoggingBuilder builder,
        string webhookUrl,
        string username)
    {
        return builder.AddDiscord(options =>
        {
            options.WebhookUrl = webhookUrl;
            options.Username = username;
        });
    }

    /// <summary>
    /// Adiciona o Discord logger ao builder de logging com webhook URL, username e avatar.
    /// </summary>
    /// <param name="builder">O ILoggingBuilder.</param>
    /// <param name="webhookUrl">URL do webhook do Discord.</param>
    /// <param name="username">Nome de usuário personalizado.</param>
    /// <param name="avatarUrl">URL do avatar personalizado.</param>
    /// <returns>O ILoggingBuilder para encadeamento.</returns>
    public static ILoggingBuilder AddDiscord(
        this ILoggingBuilder builder,
        string webhookUrl,
        string username,
        string avatarUrl)
    {
        return builder.AddDiscord(options =>
        {
            options.WebhookUrl = webhookUrl;
            options.Username = username;
            options.AvatarUrl = avatarUrl;
        });
    }
}

/// <summary>
/// Extension methods para configuração de roteamento de webhooks.
/// </summary>
public static class WebhookRoutingExtensions
{
    /// <summary>
    /// Configura roteamento de webhooks usando um builder fluente.
    /// </summary>
    public static DiscordLoggerOptions ConfigureRouting(
        this DiscordLoggerOptions options,
        Action<WebhookRoutingBuilder> configure)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new WebhookRoutingBuilder();
        configure(builder);
        builder.Validate();
        
        options.Router = builder.Build();
        return options;
    }

    /// <summary>
    /// Configura múltiplos webhooks por LogLevel.
    /// </summary>
    public static DiscordLoggerOptions ConfigureMultipleWebhooks(
        this DiscordLoggerOptions options,
        Action<WebhookConfiguration> configure)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var webhookConfig = new WebhookConfiguration();
        configure(webhookConfig);
        webhookConfig.Validate();

        options.MultipleWebhooks = webhookConfig;
        return options;
    }

    /// <summary>
    /// Habilita roteamento baseado em atributos [DiscordWebhook].
    /// </summary>
    /// <param name="options">As opções do Discord logger.</param>
    /// <param name="defaultWebhook">Webhook padrão caso nenhum atributo seja encontrado.</param>
    /// <returns>As opções para encadeamento.</returns>
    public static DiscordLoggerOptions UseAttributeRouting(
        this DiscordLoggerOptions options,
        string? defaultWebhook = null)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.EnableAttributeRouting = true;
        
        if (!string.IsNullOrWhiteSpace(defaultWebhook))
        {
            options.WebhookUrl = defaultWebhook;
        }

        return options;
    }
}
