using System.Text.Json.Serialization;

namespace Elefanti.Logging.Discord.Models;

/// <summary>
/// Representa uma mensagem de webhook do Discord.
/// </summary>
public class DiscordWebhookMessage
{
    /// <summary>
    /// Nome de usuário para sobrescrever o padrão do webhook.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// URL do avatar para sobrescrever o padrão do webhook.
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Conteúdo de texto da mensagem.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Lista de embeds (até 10).
    /// </summary>
    [JsonPropertyName("embeds")]
    public List<DiscordEmbed>? Embeds { get; set; }
}

/// <summary>
/// Representa um embed do Discord.
/// </summary>
public class DiscordEmbed
{
    /// <summary>
    /// Título do embed.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Descrição do embed.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Cor do embed (valor decimal).
    /// </summary>
    [JsonPropertyName("color")]
    public int? Color { get; set; }

    /// <summary>
    /// Campos do embed.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<EmbedField>? Fields { get; set; }

    /// <summary>
    /// Timestamp do embed.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Footer do embed.
    /// </summary>
    [JsonPropertyName("footer")]
    public EmbedFooter? Footer { get; set; }
}

/// <summary>
/// Representa um campo de embed.
/// </summary>
public class EmbedField
{
    /// <summary>
    /// Nome do campo.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor do campo.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o campo deve ser exibido inline.
    /// </summary>
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }
}

/// <summary>
/// Representa o footer de um embed.
/// </summary>
public class EmbedFooter
{
    /// <summary>
    /// Texto do footer.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// URL do ícone do footer.
    /// </summary>
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}
