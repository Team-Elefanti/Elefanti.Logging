namespace Elefanti.Logging.Discord.Attributes;

/// <summary>
/// Atributo para especificar um webhook do Discord para uma classe ou método.
/// Permite roteamento baseado em atributos.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class DiscordWebhookAttribute : Attribute
{
    /// <summary>
    /// URL do webhook do Discord.
    /// </summary>
    public string WebhookUrl { get; }

    /// <summary>
    /// Nome descritivo para o webhook (opcional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Prioridade do roteamento (maior = mais prioritário).
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// Indica se deve fazer fallback para o webhook padrão em caso de falha.
    /// </summary>
    public bool AllowFallback { get; set; } = true;

    /// <summary>
    /// Inicializa uma nova instância do atributo DiscordWebhook.
    /// </summary>
    /// <param name="webhookUrl">URL do webhook do Discord.</param>
    public DiscordWebhookAttribute(string webhookUrl)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        WebhookUrl = webhookUrl;
    }
}
