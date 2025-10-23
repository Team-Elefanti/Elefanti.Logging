namespace Elefanti.Logging.Abstractions;

/// <summary>
/// Interface para transporte de logs para destinos externos.
/// </summary>
public interface ILogTransport
{
    /// <summary>
    /// Envia a mensagem formatada para o destino externo.
    /// </summary>
    /// <param name="formattedMessage">A mensagem já formatada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SendAsync(object formattedMessage, CancellationToken cancellationToken = default);
}
