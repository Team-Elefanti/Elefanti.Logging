using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Models;

/// <summary>
/// Representa uma entrada de log estruturada.
/// </summary>
public sealed class LogEntry
{
    /// <summary>
    /// Nível de log.
    /// </summary>
    public LogLevel LogLevel { get; init; }

    /// <summary>
    /// ID do evento de log.
    /// </summary>
    public EventId EventId { get; init; }

    /// <summary>
    /// Categoria do log (geralmente o nome completo da classe).
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Mensagem de log.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Exceção associada ao log, se houver.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Timestamp do log em UTC.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Scopes ativos no momento do log.
    /// </summary>
    public IReadOnlyList<object> Scopes { get; init; } = Array.Empty<object>();

    /// <summary>
    /// Dados estruturados (parâmetros) do log.
    /// </summary>
    public IReadOnlyDictionary<string, object?> State { get; init; } = new Dictionary<string, object?>();
}
