using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;
using Elefanti.Logging.Discord;
using ConsoleLoggingSample.Helpers;
using ConsoleLoggingSample.Services;

namespace ConsoleLoggingSample.Examples;

public static class MultipleWebhooksExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine("?       [2] Múltiplos Webhooks - Canais por LogLevel          ?");
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine();

        var builder = Host.CreateApplicationBuilder();

        var webhookCritical = ConfigurationHelper.GetWebhookCritical();
        var webhookErrors = ConfigurationHelper.GetWebhookErrors();
        var webhookWarnings = ConfigurationHelper.GetWebhookWarnings();
        var webhookInfo = ConfigurationHelper.GetWebhookInfo();
        var webhookDefault = ConfigurationHelper.GetWebhookDefault();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();

            logging.AddDiscord(options =>
            {
                options.Username = $"{UnicodeEmojis.Target} Multi-Webhook Logger";
                options.MinimumLevel = LogLevel.Information;
                options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();

                options.ConfigureMultipleWebhooks(webhooks =>
                {
                    webhooks.SetWebhook(LogLevel.Critical, webhookCritical);
                    webhooks.SetWebhook(LogLevel.Error, webhookErrors);
                    webhooks.SetWebhook(LogLevel.Warning, webhookWarnings);
                    webhooks.SetWebhook(LogLevel.Information, webhookInfo);
                    webhooks.DefaultWebhook = webhookDefault;
                });
            });

            logging.SetMinimumLevel(LogLevel.Trace);
        });

        builder.Services.AddSingleton<MultipleWebhooksApp>();

        var host = builder.Build();
        var app = host.Services.GetRequiredService<MultipleWebhooksApp>();
        await app.RunAsync();
    }
}
