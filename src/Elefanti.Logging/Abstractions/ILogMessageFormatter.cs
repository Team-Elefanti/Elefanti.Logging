namespace Elefanti.Logging.Abstractions;

/// <summary>
/// Interface para formatação de mensagens de log.
/// </summary>
public interface ILogMessageFormatter
{
    /// <summary>
    /// Formata uma entrada de log para o formato específico do provedor.
    /// </summary>
    /// <param name="logEntry">A entrada de log a ser formatada.</param>
    /// <returns>O objeto formatado pronto para envio.</returns>
    object Format(Models.LogEntry logEntry);
}
