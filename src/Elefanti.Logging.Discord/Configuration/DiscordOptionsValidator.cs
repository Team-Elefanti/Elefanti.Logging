namespace Elefanti.Logging.Discord.Configuration;

/// <summary>
/// Validador de opções do Discord Logger.
/// </summary>
public static class DiscordOptionsValidator
{
    /// <summary>
    /// Valida as opções de configuração do Discord Logger.
    /// </summary>
    public static void Validate(DiscordLoggerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // Se há Router configurado, valida-o (Router tem seu próprio DefaultWebhook)
        if (options.Router != null)
        {
            // Router já valida suas próprias regras e webhook padrão
            // Não precisa de WebhookUrl nas options
        }
        // Se há configuração de múltiplos webhooks, valida ela
        else if (options.MultipleWebhooks != null)
        {
            options.MultipleWebhooks.Validate();
        }
        // Senão, valida o webhook único (obrigatório)
        else
        {
            if (string.IsNullOrWhiteSpace(options.WebhookUrl))
            {
                throw new ArgumentException(
                    "WebhookUrl é obrigatório quando nem Router nem MultipleWebhooks estão configurados.", 
                    nameof(options.WebhookUrl));
            }

            if (!Uri.TryCreate(options.WebhookUrl, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("WebhookUrl deve ser uma URL válida.", nameof(options.WebhookUrl));
            }

            if (!uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.EndsWith("discordapp.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("WebhookUrl deve ser um webhook do Discord.", nameof(options.WebhookUrl));
            }
        }

        if (options.MaxRetryAttempts < 0)
        {
            throw new ArgumentException("MaxRetryAttempts deve ser maior ou igual a 0.", nameof(options.MaxRetryAttempts));
        }

        if (options.TimeoutSeconds <= 0)
        {
            throw new ArgumentException("TimeoutSeconds deve ser maior que 0.", nameof(options.TimeoutSeconds));
        }
    }
}
