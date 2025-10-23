using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;
using Elefanti.Logging.Discord;
using ConsoleLoggingSample.Helpers;
using ConsoleLoggingSample.Services;

namespace ConsoleLoggingSample.Examples;

public static class AttributeRoutingExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine("?     [5] Attribute Routing - Roteamento por Atributos        ?");
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine();

        var builder = Host.CreateApplicationBuilder();

        var webhookDefault = ConfigurationHelper.GetWebhookDefault();
        var webhookPayments = ConfigurationHelper.GetWebhookPayments();
        var webhookAuth = ConfigurationHelper.GetWebhookAuth();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();

            logging.AddDiscord(options =>
            {
                options.UseAttributeRouting(webhookDefault);
                options.Username = $"{UnicodeEmojis.Tag} Attribute Router Logger";
                options.MinimumLevel = LogLevel.Information;
                options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();
            });

            logging.SetMinimumLevel(LogLevel.Information);
        });

        // Registra serviços com atributos
        builder.Services.AddSingleton<PaymentServiceWithAttribute>(sp => 
            new PaymentServiceWithAttribute(
                sp.GetRequiredService<ILogger<PaymentServiceWithAttribute>>(),
                webhookPayments));
        
        builder.Services.AddSingleton<AuthServiceWithAttribute>(sp =>
            new AuthServiceWithAttribute(
                sp.GetRequiredService<ILogger<AuthServiceWithAttribute>>(),
                webhookAuth));

        var host = builder.Build();

        Console.WriteLine($"{UnicodeEmojis.Play} Demonstrando attribute routing...\n");
        Console.WriteLine($"{UnicodeEmojis.Tag} Classes decoradas com [DiscordWebhook] attribute:");
        Console.WriteLine();

        // Executa exemplos
        var paymentService = host.Services.GetRequiredService<PaymentServiceWithAttribute>();
        Console.WriteLine($"1. PaymentService {UnicodeEmojis.Arrow} Webhook de Payments:");
        await paymentService.ProcessPayment(150.75m);
        await Task.Delay(1000);

        var authService = host.Services.GetRequiredService<AuthServiceWithAttribute>();
        Console.WriteLine($"\n2. AuthService {UnicodeEmojis.Arrow} Webhook de Auth:");
        await authService.Login("joao.silva@example.com");
        await Task.Delay(1000);

        Console.WriteLine($"\n{UnicodeEmojis.Success} Exemplo de attribute routing concluído!");
        Console.WriteLine($"{UnicodeEmojis.Lightbulb} Dica: Use [DiscordWebhook] para rotear logs automaticamente");
        Console.WriteLine("   sem precisar configurar regras manualmente!");
    }
}
