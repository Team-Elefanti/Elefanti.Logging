using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class FileAttachmentsApp
{
    private readonly ILogger<FileAttachmentsApp> _logger;

    public FileAttachmentsApp(ILogger<FileAttachmentsApp> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"{UnicodeEmojis.Play} Demonstrando anexos de arquivo...\n");
        Console.WriteLine($"{UnicodeEmojis.Attachment} Stack traces grandes são enviadas como arquivo .txt:");
        Console.WriteLine();

        // Exception simples (pode ser inline)
        Console.WriteLine("1. Exception pequena (inline no embed):");
        try
        {
            throw new ArgumentException("Argumento inválido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception pequena");
        }
        await Task.Delay(1500);

        // Exception com InnerException (mais longa)
        Console.WriteLine("\n2. Exception com InnerException (anexo):");
        try
        {
            try
            {
                throw new InvalidOperationException("Inner exception: Operação inválida no banco de dados");
            }
            catch (Exception inner)
            {
                throw new ApplicationException("Outer exception: Falha ao processar requisição", inner);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Exception com inner exception {UnicodeEmojis.Arrow} arquivo anexo");
        }
        await Task.Delay(1500);

        // Simula stack trace muito longa
        Console.WriteLine("\n3. Exception complexa com stack trace longa:");
        try
        {
            SimulateDeepStackTrace(10);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Stack trace profunda {UnicodeEmojis.Arrow} arquivo .txt anexado");
        }
        await Task.Delay(1500);

        Console.WriteLine($"\n{UnicodeEmojis.Success} Exemplo de anexos concluído!");
        Console.WriteLine($"{UnicodeEmojis.Lightbulb} Configurações:");
        Console.WriteLine("   - EnableAttachments = true");
        Console.WriteLine("   - AttachmentSizeThreshold = 1000 caracteres");
        Console.WriteLine("   - AlwaysAttachStackTraces = true");
    }

    private void SimulateDeepStackTrace(int depth)
    {
        if (depth == 0)
        {
            throw new InvalidOperationException(
                "Simulação de stack trace profunda. " +
                "Esta exceção foi gerada através de uma cadeia de chamadas " +
                "recursivas para demonstrar o anexo de stack traces longas. " +
                "O sistema detecta automaticamente quando uma stack trace " +
                "é muito grande e a envia como arquivo .txt anexo ao Discord, " +
                "mantendo um resumo no embed principal.");
        }
        SimulateDeepStackTrace(depth - 1);
    }
}
