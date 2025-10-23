using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;
using Elefanti.Logging.Discord;
using ConsoleLoggingSample.Helpers;
using ConsoleLoggingSample.Services;

namespace ConsoleLoggingSample.Examples;

public static class BasicExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("????????????????????????????????????????????????????????????????");
        Console.WriteLine("?          [1] Exemplo Básico - Features Principais            ?");
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
                options.Username = $"{UnicodeEmojis.Elephant} Elefanti Logger";
                options.AvatarUrl = "https://cdn.discordapp.com/embed/avatars/0.png";
                options.MinimumLevel = LogLevel.Information;
                options.IncludeTimestamp = true;
                options.IncludeCategory = true;
                options.IncludeEventId = true;
                options.IncludeScopes = true;
                options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();
            });

            logging.SetMinimumLevel(LogLevel.Trace);
        });

        builder.Services.AddSingleton<BasicExampleApp>();

        var host = builder.Build();
        var app = host.Services.GetRequiredService<BasicExampleApp>();
        await app.RunAsync();
    }
}
