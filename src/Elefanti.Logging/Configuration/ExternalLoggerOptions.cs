using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Configuration;

/// <summary>
/// Opções base para provedores externos de logging.
/// </summary>
public class ExternalLoggerOptions
{
    /// <summary>
    /// Nível mínimo de log.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Indica se deve incluir scopes nas mensagens de log.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir o Event ID nas mensagens de log.
    /// </summary>
    public bool IncludeEventId { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir o timestamp nas mensagens de log.
    /// </summary>
    public bool IncludeTimestamp { get; set; } = true;

    /// <summary>
    /// Indica se deve incluir a categoria (nome da classe) nas mensagens de log.
    /// </summary>
    public bool IncludeCategory { get; set; } = true;
}
