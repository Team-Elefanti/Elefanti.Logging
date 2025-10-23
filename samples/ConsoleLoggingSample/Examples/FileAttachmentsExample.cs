using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;
using Elefanti.Logging.Discord;
using ConsoleLoggingSample.Helpers;
using ConsoleLoggingSample.Services;

namespace ConsoleLoggingSample.Examples;

public static class FileAttachmentsExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine("?       [4] Anexos de Arquivo - Stack Traces Grandes          ?");
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine();

        var builder = Host.CreateApplicationBuilder();

        var webhookUrl = ConfigurationHelper.GetWebhookUrl();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();

            logging.AddDiscord(options =>
            {
                options.WebhookUrl = webhookUrl;
                options.Username = $"{UnicodeEmojis.Attachment} Attachment Logger";
                options.MinimumLevel = LogLevel.Information;
                options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();
                
                // Configuração de anexos
                options.EnableAttachments = true;
                options.AttachmentSizeThreshold = 1000; // Menor threshold para demonstração
                options.AlwaysAttachStackTraces = true;
            });

            logging.SetMinimumLevel(LogLevel.Trace);
        });

        builder.Services.AddSingleton<FileAttachmentsApp>();

        var host = builder.Build();
        var app = host.Services.GetRequiredService<FileAttachmentsApp>();
        await app.RunAsync();
    }
}
