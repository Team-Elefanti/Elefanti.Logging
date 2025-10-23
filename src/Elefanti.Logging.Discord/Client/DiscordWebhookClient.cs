using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Elefanti.Logging.Discord.Configuration;
using Elefanti.Logging.Discord.Models;

namespace Elefanti.Logging.Discord.Client;

/// <summary>
/// Cliente HTTP para envio de logs via Discord Webhook.
/// </summary>
public sealed class DiscordWebhookClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly DiscordLoggerOptions _options;
    private readonly SemaphoreSlim _rateLimitSemaphore;
    private DateTime _lastRequestTime = DateTime.MinValue;
    private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(2);
    private const int MaxRequestsPerWindow = 5;
    private int _requestCount = 0;
    private bool _disposed;

    // JsonSerializerOptions compartilhado para melhor performance e garantia de encoding correto
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // Preserva emojis e caracteres Unicode
    };

    public DiscordWebhookClient(HttpClient httpClient, DiscordLoggerOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _rateLimitSemaphore = new SemaphoreSlim(1, 1);

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <summary>
    /// Envia uma mensagem para o webhook do Discord com retry automático.
    /// </summary>
    /// <param name="message">Mensagem a ser enviada.</param>
    /// <param name="webhookUrl">URL do webhook (opcional, usa configuração padrão se não fornecido).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    public async Task SendAsync(DiscordWebhookMessage message, string? webhookUrl = null, CancellationToken cancellationToken = default)
    {
        await SendAsync(message, null, webhookUrl, cancellationToken);
    }

    /// <summary>
    /// Envia uma mensagem para o webhook do Discord com anexo opcional.
    /// </summary>
    /// <param name="message">Mensagem a ser enviada.</param>
    /// <param name="attachment">Anexo opcional (nome do arquivo e conteúdo).</param>
    /// <param name="webhookUrl">URL do webhook (opcional, usa configuração padrão se não fornecido).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    public async Task SendAsync(
        DiscordWebhookMessage message, 
        (string filename, byte[] content)? attachment = null,
        string? webhookUrl = null, 
        CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DiscordWebhookClient));
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Usa o webhook fornecido ou o padrão da configuração
        var targetWebhook = webhookUrl ?? _options.WebhookUrl;
        
        if (string.IsNullOrWhiteSpace(targetWebhook))
        {
            throw new InvalidOperationException("Nenhum webhook configurado para envio.");
        }

        int attempt = 0;
        Exception? lastException = null;

        while (attempt <= _options.MaxRetryAttempts)
        {
            try
            {
                await ApplyRateLimitAsync(cancellationToken);
                
                HttpResponseMessage response;

                // Se há anexo, usa multipart/form-data
                if (attachment.HasValue)
                {
                    response = await SendWithAttachmentAsync(targetWebhook, message, attachment.Value, cancellationToken);
                }
                else
                {
                    // Envio normal via JSON com encoding UTF-8 correto
                    response = await _httpClient.PostAsJsonAsync(
                        targetWebhook,
                        message,
                        JsonOptions,
                        cancellationToken);
                }

                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                // Se for rate limit (429), espera mais tempo
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    attempt++;
                    continue;
                }

                // Se for erro do cliente (4xx), não tenta novamente
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    return;
                }

                // Erro de servidor (5xx), tenta novamente
                lastException = new HttpRequestException($"Discord webhook retornou status {response.StatusCode}");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                lastException = ex;
            }

            if (attempt < _options.MaxRetryAttempts)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Backoff exponencial
                await Task.Delay(delay, cancellationToken);
            }

            attempt++;
        }

        // Silenciosamente ignora falhas após todas as tentativas
        // O logging não deve quebrar a aplicação
    }

    private async Task<HttpResponseMessage> SendWithAttachmentAsync(
        string webhookUrl,
        DiscordWebhookMessage message,
        (string filename, byte[] content) attachment,
        CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();

        // Adiciona o payload JSON com encoding UTF-8 correto
        var jsonPayload = JsonSerializer.Serialize(message, JsonOptions);

        content.Add(new StringContent(jsonPayload, Encoding.UTF8, "application/json"), "payload_json");

        // Adiciona o arquivo
        var fileContent = new ByteArrayContent(attachment.content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", attachment.filename);

        return await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
    }

    private async Task ApplyRateLimitAsync(CancellationToken cancellationToken)
    {
        await _rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            
            // Reset do contador se passou a janela de tempo
            if (now - _lastRequestTime > _rateLimitWindow)
            {
                _requestCount = 0;
                _lastRequestTime = now;
            }

            // Se atingiu o limite, espera até a próxima janela
            if (_requestCount >= MaxRequestsPerWindow)
            {
                var waitTime = _rateLimitWindow - (now - _lastRequestTime);
                if (waitTime > TimeSpan.Zero)
                {
                    await Task.Delay(waitTime, cancellationToken);
                }
                _requestCount = 0;
                _lastRequestTime = DateTime.UtcNow;
            }

            _requestCount++;
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _rateLimitSemaphore?.Dispose();
    }
}