using ConsoleLoggingSample.Examples;
using Elefanti.Logging.Discord;

// ============================================
// 🚀 Elefanti.Logging - Discord Provider Demo
// ============================================

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║    Elefanti.Logging - Discord Provider Sample Application   ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Escolha o exemplo que deseja executar:");
Console.WriteLine();
Console.WriteLine("[1] - Exemplo Básico (níveis de log, structured logging, scopes)");
Console.WriteLine("[2] - Múltiplos Webhooks (webhooks diferentes por LogLevel)");
Console.WriteLine("[3] - Routing Avançado (roteamento inteligente por regras)");
Console.WriteLine("[4] - Anexos de Arquivo (stack traces grandes como arquivo)");
Console.WriteLine("[5] - Attribute Routing (roteamento por atributos nas classes)");
Console.WriteLine("[6] - Executar TODOS os exemplos");
Console.WriteLine("[0] - Sair");
Console.WriteLine();
Console.Write("Opção: ");

var option = Console.ReadLine();

switch (option)
{
    case "1":
        await BasicExample.RunAsync();
        break;
    case "2":
        await MultipleWebhooksExample.RunAsync();
        break;
    case "3":
        await AdvancedRoutingExample.RunAsync();
        break;
    case "4":
        await FileAttachmentsExample.RunAsync();
        break;
    case "5":
        await AttributeRoutingExample.RunAsync();
        break;
    case "6":
        Console.WriteLine($"\n{UnicodeEmojis.Target} Executando TODOS os exemplos...\n");
        await BasicExample.RunAsync();
        Console.WriteLine("\n" + new string('═', 60) + "\n");
        await MultipleWebhooksExample.RunAsync();
        Console.WriteLine("\n" + new string('═', 60) + "\n");
        await AdvancedRoutingExample.RunAsync();
        Console.WriteLine("\n" + new string('═', 60) + "\n");
        await FileAttachmentsExample.RunAsync();
        Console.WriteLine("\n" + new string('═', 60) + "\n");
        await AttributeRoutingExample.RunAsync();
        break;
    case "0":
        Console.WriteLine($"{UnicodeEmojis.Wave} Até logo!");
        return;
    default:
        Console.WriteLine($"{UnicodeEmojis.Error} Opção inválida!");
        return;
}

Console.WriteLine($"\n{UnicodeEmojis.Success} Execução finalizada! Pressione qualquer tecla para sair...");
Console.ReadKey();
