using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class AuthServiceWithAttribute
{
    private readonly ILogger<AuthServiceWithAttribute> _logger;
    private readonly string _webhookUrl;

    public AuthServiceWithAttribute(
        ILogger<AuthServiceWithAttribute> logger,
        string webhookUrl)
    {
        _logger = logger;
        _webhookUrl = webhookUrl;
    }

    public async Task Login(string email)
    {
        using (_logger.BeginScope("User Authentication"))
        {
            _logger.LogInformation($"{UnicodeEmojis.Auth} Tentativa de login para {{Email}}", email);
            await Task.Delay(500);

            _logger.LogInformation($"{UnicodeEmojis.Success} Login bem-sucedido para {{Email}}", email);
            await Task.Delay(500);

            _logger.LogInformation($"{UnicodeEmojis.Token} Token gerado e sessão criada");
            await Task.Delay(300);
        }
    }
}
