using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord;

namespace ConsoleLoggingSample.Services;

public class PaymentServiceWithAttribute
{
    private readonly ILogger<PaymentServiceWithAttribute> _logger;
    private readonly string _webhookUrl;

    public PaymentServiceWithAttribute(
        ILogger<PaymentServiceWithAttribute> logger,
        string webhookUrl)
    {
        _logger = logger;
        _webhookUrl = webhookUrl;
    }

    public async Task ProcessPayment(decimal amount)
    {
        using (_logger.BeginScope("Payment Processing"))
        {
            _logger.LogInformation($"{UnicodeEmojis.Payment} Iniciando processamento de pagamento de {{Amount:C}}", amount);
            await Task.Delay(500);

            _logger.LogInformation($"{UnicodeEmojis.Success} Pagamento de {{Amount:C}} processado com sucesso", amount);
            await Task.Delay(500);

            _logger.LogInformation($"{UnicodeEmojis.Email} Enviando confirmação por email...");
            await Task.Delay(300);
        }
    }
}
