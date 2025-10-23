using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;
using Elefanti.Logging.Discord;
using ConsoleLoggingSample.Helpers;
using ConsoleLoggingSample.Services;

namespace ConsoleLoggingSample.Examples;

public static class AdvancedRoutingExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine("?        [3] Routing Avançado - Regras Inteligentes           ?");
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine();

        var builder = Host.CreateApplicationBuilder();

        var webhookCritical = ConfigurationHelper.GetWebhookCritical();
        var webhookErrors = ConfigurationHelper.GetWebhookErrors();
        var webhookWarnings = ConfigurationHelper.GetWebhookWarnings();
        var webhookPayments = ConfigurationHelper.GetWebhookPayments();
        var webhookDefault = ConfigurationHelper.GetWebhookDefault();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();

            logging.AddDiscord(options =>
            {
                options.Username = $"{UnicodeEmojis.Target} Smart Router Logger";
                options.MinimumLevel = LogLevel.Information;
                options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();

                options.ConfigureRouting(routing =>
                {
                    routing.RouteLevel(LogLevel.Critical, webhookCritical, priority: 1000);
                    routing.RouteCategory("*.Payment.*", webhookPayments, priority: 900);
                    routing.RouteCategory("*.Auth.*", webhookErrors, priority: 900);
                    routing.RouteExceptionType<InvalidOperationException>(webhookErrors, priority: 800);
                    routing.RouteExceptionType<ArgumentException>(webhookWarnings, priority: 800);
                    routing.RouteEventId(5000, webhookCritical, priority: 850);
                    routing.RouteEventName("SecurityAlert", webhookCritical, priority: 850);
                    routing.RouteWhen(
                        log => log.Message.Contains("SQL") || log.Message.Contains("Database"),
                        webhookErrors,
                        priority: 750,
                        description: "Database-related logs"
                    );
                    routing.RouteDefault(webhookDefault);
                });
            });

            logging.SetMinimumLevel(LogLevel.Trace);
        });

        builder.Services.AddSingleton<AdvancedRoutingApp>();

        var host = builder.Build();
        var app = host.Services.GetRequiredService<AdvancedRoutingApp>();
        await app.RunAsync();
    }
}
