using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Routing;
using Elefanti.Logging.Models;

namespace Elefanti.Logging.Discord.Configuration;

/// <summary>
/// Opções de configuração para o Discord Logger.
/// </summary>
public class DiscordLoggerOptions
{
    /// <summary>
    /// URL do webhook do Discord (usado quando não há configuração de múltiplos webhooks).
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Configuração de múltiplos webhooks por LogLevel.
    /// </summary>
    public WebhookConfiguration? MultipleWebhooks { get; set; }

    /// <summary>
    /// Router de webhooks baseado em regras customizadas.
    /// Tem prioridade sobre MultipleWebhooks.
    /// </summary>
    public WebhookRouter? Router { get; set; }

    /// <summary>
    /// Habilita roteamento baseado em atributos [DiscordWebhook].
    /// </summary>
    public bool EnableAttributeRouting { get; set; } = false;

    /// <summary>
    /// Nome de usuário personalizado para o bot no Discord.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// URL do avatar personalizado para o bot no Discord.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Nível mínimo de log.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Indica se deve incluir scopes nas mensagens.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir timestamp nas mensagens.
    /// </summary>
    public bool IncludeTimestamp { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir a categoria nas mensagens.
    /// </summary>
    public bool IncludeCategory { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir o Event ID nas mensagens.
    /// </summary>
    public bool IncludeEventId { get; set; } = true;

    /// <summary>
    /// Número máximo de tentativas de envio.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Timeout para requisições HTTP em segundos.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Emojis personalizados por nível de log.
    /// </summary>
    public Dictionary<LogLevel, string> CustomEmojis { get; set; } = new();

    /// <summary>
    /// Tamanho mínimo de mensagem (em caracteres) para anexar como arquivo.
    /// Padrão: 2000 caracteres.
    /// </summary>
    public int AttachmentSizeThreshold { get; set; } = 2000;

    /// <summary>
    /// Indica se deve sempre anexar stack traces completas como arquivo,
    /// independentemente do tamanho.
    /// </summary>
    public bool AlwaysAttachStackTraces { get; set; } = false;

    /// <summary>
    /// Indica se deve habilitar anexos de arquivos para mensagens grandes.
    /// </summary>
    public bool EnableAttachments { get; set; } = true;

    /// <summary>
    /// Obtém o webhook apropriado para o LogEntry especificado.
    /// </summary>
    public string GetWebhookForLog(LogEntry logEntry)
    {
        // 1. Prioridade: Router (regras customizadas)
        if (Router != null)
        {
            var routedWebhook = Router.Route(logEntry);
            if (!string.IsNullOrWhiteSpace(routedWebhook))
            {
                return routedWebhook;
            }
        }

        // 2. Segunda prioridade: Múltiplos webhooks por LogLevel
        if (MultipleWebhooks != null)
        {
            var webhook = MultipleWebhooks.GetWebhookForLevel(logEntry.LogLevel);
            if (!string.IsNullOrWhiteSpace(webhook))
            {
                return webhook;
            }
        }

        // 3. Fallback: Webhook único
        return WebhookUrl;
    }

    /// <summary>
    /// Obtém o webhook apropriado para o LogLevel especificado (backward compatibility).
    /// </summary>
    public string GetWebhookForLevel(LogLevel logLevel)
    {
        // Se há configuração de múltiplos webhooks, usa ela
        if (MultipleWebhooks != null)
        {
            var webhook = MultipleWebhooks.GetWebhookForLevel(logLevel);
            if (!string.IsNullOrWhiteSpace(webhook))
            {
                return webhook;
            }
        }

        // Fallback para o webhook único
        return WebhookUrl;
    }
}
