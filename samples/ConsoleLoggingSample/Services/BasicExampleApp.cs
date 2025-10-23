using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class BasicExampleApp
{
    private readonly ILogger<BasicExampleApp> _logger;

    public BasicExampleApp(ILogger<BasicExampleApp> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"{UnicodeEmojis.Play} Demonstrando features básicas...\n");

        // Diferentes níveis de log
        Console.WriteLine($"{UnicodeEmojis.Chart} Níveis de Log:");
        _logger.LogTrace("Trace - debugging muito detalhado");
        await Task.Delay(500);
        _logger.LogDebug("Debug - informações para desenvolvimento");
        await Task.Delay(500);
        _logger.LogInformation("Information - fluxo normal da aplicação");
        await Task.Delay(500);
        _logger.LogWarning("Warning - algo não esperado");
        await Task.Delay(500);
        _logger.LogError("Error - algo deu errado");
        await Task.Delay(500);
        _logger.LogCritical("Critical - erro grave!");
        await Task.Delay(1000);

        // Structured Logging
        Console.WriteLine($"\n{UnicodeEmojis.Document} Structured Logging:");
        _logger.LogInformation(
            "Usuário {UserId} '{UserName}' realizou compra de {Amount:C}",
            12345, "João Silva", 150.50m);
        await Task.Delay(1000);

        // Event IDs
        Console.WriteLine($"\n{UnicodeEmojis.Bookmark} Event IDs:");
        _logger.LogInformation(new EventId(1001, "UserLogin"), "Login bem-sucedido");
        await Task.Delay(1000);

        // Scopes
        Console.WriteLine($"\n{UnicodeEmojis.Trace} Scopes:");
        using (_logger.BeginScope("Pedido #{OrderId}", 9876))
        {
            _logger.LogInformation("Validando pedido...");
            await Task.Delay(500);
            _logger.LogInformation("Processando pagamento...");
            await Task.Delay(500);
            _logger.LogInformation("Pedido concluído!");
        }
        await Task.Delay(1000);

        // Exception
        Console.WriteLine($"\n{UnicodeEmojis.Warning} Exceções:");
        try
        {
            throw new InvalidOperationException("Operação inválida simulada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar operação");
        }

        await Task.Delay(1000);
        Console.WriteLine($"\n{UnicodeEmojis.Success} Exemplo básico concluído!");
    }
}
