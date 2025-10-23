using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class MultipleWebhooksApp
{
    private readonly ILogger<MultipleWebhooksApp> _logger;

    public MultipleWebhooksApp(ILogger<MultipleWebhooksApp> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"{UnicodeEmojis.Play} Demonstrando múltiplos webhooks...\n");
        Console.WriteLine($"{UnicodeEmojis.Bookmark} Cada nível de log vai para um canal diferente:");
        Console.WriteLine();

        _logger.LogInformation($"Log Information {UnicodeEmojis.Arrow} Canal de Info");
        await Task.Delay(800);

        _logger.LogWarning($"Log Warning {UnicodeEmojis.Arrow} Canal de Warnings");
        await Task.Delay(800);

        _logger.LogError($"Log Error {UnicodeEmojis.Arrow} Canal de Errors");
        await Task.Delay(800);

        _logger.LogCritical($"Log Critical {UnicodeEmojis.Arrow} Canal de Critical");
        await Task.Delay(800);

        _logger.LogDebug($"Log Debug {UnicodeEmojis.Arrow} Canal Default (fallback)");
        await Task.Delay(1000);

        Console.WriteLine($"\n{UnicodeEmojis.Success} Exemplo de múltiplos webhooks concluído!");
        Console.WriteLine($"{UnicodeEmojis.Lightbulb} Dica: Configure webhooks diferentes nas variáveis de ambiente:");
        Console.WriteLine("   - DISCORD_WEBHOOK_CRITICAL");
        Console.WriteLine("   - DISCORD_WEBHOOK_ERRORS");
        Console.WriteLine("   - DISCORD_WEBHOOK_WARNINGS");
        Console.WriteLine("   - DISCORD_WEBHOOK_INFO");
        Console.WriteLine("   - DISCORD_WEBHOOK_DEFAULT");
    }
}
