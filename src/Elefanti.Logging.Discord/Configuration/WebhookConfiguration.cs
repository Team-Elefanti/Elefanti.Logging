using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Discord.Configuration;

/// <summary>
/// Configuração de webhooks por LogLevel.
/// </summary>
public class WebhookConfiguration
{
    /// <summary>
    /// Webhooks específicos por LogLevel.
    /// </summary>
    public Dictionary<LogLevel, string> WebhooksByLevel { get; set; } = new();

    /// <summary>
    /// Webhook padrão (fallback) quando não há webhook específico para o nível.
    /// </summary>
    public string? DefaultWebhook { get; set; }

    /// <summary>
    /// Obtém o webhook apropriado para o LogLevel especificado.
    /// </summary>
    public string? GetWebhookForLevel(LogLevel logLevel)
    {
        if (WebhooksByLevel.TryGetValue(logLevel, out var webhook))
        {
            return webhook;
        }

        return DefaultWebhook;
    }

    /// <summary>
    /// Define um webhook para um LogLevel específico.
    /// </summary>
    public WebhookConfiguration SetWebhook(LogLevel logLevel, string webhookUrl)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazio", nameof(webhookUrl));
        }

        WebhooksByLevel[logLevel] = webhookUrl;
        return this;
    }

    /// <summary>
    /// Valida a configuração de webhooks.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DefaultWebhook) && !WebhooksByLevel.Any())
        {
            throw new InvalidOperationException(
                "Pelo menos um webhook deve ser configurado (DefaultWebhook ou webhooks específicos por nível).");
        }

        foreach (var webhook in WebhooksByLevel.Values.Concat(new[] { DefaultWebhook }).Where(w => !string.IsNullOrWhiteSpace(w)))
        {
            if (!Uri.TryCreate(webhook, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Webhook URL inválida: {webhook}");
            }

            if (!uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.EndsWith("discordapp.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Webhook URL deve ser do Discord: {webhook}");
            }
        }
    }
}
