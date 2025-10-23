using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class AdvancedRoutingApp
{
    private readonly ILogger<AdvancedRoutingApp> _logger;

    public AdvancedRoutingApp(ILogger<AdvancedRoutingApp> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"{UnicodeEmojis.Play} Demonstrando routing avançado...\n");
        Console.WriteLine($"{UnicodeEmojis.Target} Logs roteados por regras customizadas:");
        Console.WriteLine();

        // Por LogLevel (prioridade alta)
        Console.WriteLine("1. Por LogLevel:");
        _logger.LogCritical($"Critical {UnicodeEmojis.Arrow} webhook de Critical (prioridade 1000)");
        await Task.Delay(800);

        // Por Event ID
        Console.WriteLine("\n2. Por Event ID:");
        _logger.LogWarning(new EventId(5000, "SystemFailure"), $"Event ID 5000 {UnicodeEmojis.Arrow} webhook Critical");
        await Task.Delay(800);

        // Por Event Name
        Console.WriteLine("\n3. Por Event Name:");
        _logger.LogWarning(new EventId(9001, "SecurityAlert"), $"SecurityAlert {UnicodeEmojis.Arrow} webhook Critical");
        await Task.Delay(800);

        // Por tipo de exceção
        Console.WriteLine("\n4. Por tipo de exceção:");
        try
        {
            throw new InvalidOperationException("Operação inválida");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"InvalidOperationException {UnicodeEmojis.Arrow} webhook Errors");
        }
        await Task.Delay(800);

        // Condicional (mensagem contém SQL)
        Console.WriteLine("\n5. Por conteúdo da mensagem:");
        _logger.LogError($"Erro ao executar SQL query {UnicodeEmojis.Arrow} webhook Errors (regra customizada)");
        await Task.Delay(800);

        // Fallback
        Console.WriteLine("\n6. Fallback (sem regra específica):");
        _logger.LogInformation($"Log genérico {UnicodeEmojis.Arrow} webhook Default");
        await Task.Delay(1000);

        Console.WriteLine($"\n{UnicodeEmojis.Success} Exemplo de routing avançado concluído!");
        Console.WriteLine($"{UnicodeEmojis.Lightbulb} Regras avaliadas por prioridade (maior = primeira)");
    }
}
