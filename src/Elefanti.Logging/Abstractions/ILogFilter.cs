namespace Elefanti.Logging.Abstractions;

/// <summary>
/// Interface para filtragem de logs.
/// </summary>
public interface ILogFilter
{
    /// <summary>
    /// Determina se uma entrada de log deve ser processada.
    /// </summary>
    /// <param name="logEntry">A entrada de log a ser avaliada.</param>
    /// <returns>True se o log deve ser processado; caso contrário, false.</returns>
    bool ShouldLog(Models.LogEntry logEntry);
}
