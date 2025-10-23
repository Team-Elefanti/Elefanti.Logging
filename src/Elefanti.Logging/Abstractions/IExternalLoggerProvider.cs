namespace Elefanti.Logging.Abstractions;

/// <summary>
/// Interface base para provedores de logging externos.
/// </summary>
public interface IExternalLoggerProvider : IDisposable
{
    /// <summary>
    /// Nome do provedor de logging.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Envia uma entrada de log para o destino externo.
    /// </summary>
    /// <param name="logEntry">A entrada de log a ser enviada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SendLogAsync(Models.LogEntry logEntry, CancellationToken cancellationToken = default);
}
