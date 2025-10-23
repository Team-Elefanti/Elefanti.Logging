using Elefanti.Logging.Discord.Client;
using Elefanti.Logging.Discord.Configuration;
using Elefanti.Logging.Discord.Formatting;
using Elefanti.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Discord.Providers;

/// <summary>
/// Provedor de logger do Discord que implementa ILoggerProvider.
/// </summary>
public sealed class DiscordLoggerProvider : ILoggerProvider
{
    private readonly DiscordWebhookClient _client;
    private readonly DiscordMessageFormatter _formatter;
    private readonly DiscordLoggerOptions _options;
    private readonly IExternalScopeProvider _scopeProvider;
    private bool _disposed;

    public DiscordLoggerProvider(
        DiscordWebhookClient client,
        DiscordMessageFormatter formatter,
        DiscordLoggerOptions options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _scopeProvider = new LoggerExternalScopeProvider();
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DiscordLoggerProvider));
        }

        return new DiscordLogger(categoryName, _client, _formatter, _options, _scopeProvider);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Logger específico do Discord.
/// </summary>
internal sealed class DiscordLogger : ILogger
{
    private readonly string _categoryName;
    private readonly DiscordWebhookClient _client;
    private readonly DiscordMessageFormatter _formatter;
    private readonly DiscordLoggerOptions _options;
    private readonly IExternalScopeProvider _scopeProvider;

    public DiscordLogger(
        string categoryName,
        DiscordWebhookClient client,
        DiscordMessageFormatter formatter,
        DiscordLoggerOptions options,
        IExternalScopeProvider scopeProvider)
    {
        _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None && logLevel >= _options.MinimumLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var message = formatter(state, exception);

        if (string.IsNullOrEmpty(message) && exception == null)
        {
            return;
        }

        // Captura scopes
        var scopes = new List<object>();
        _scopeProvider.ForEachScope((scope, list) => list.Add(scope), scopes);

        // Captura state estruturado
        var stateDict = new Dictionary<string, object?>();
        if (state is IEnumerable<KeyValuePair<string, object?>> stateValues)
        {
            foreach (var kvp in stateValues)
            {
                stateDict[kvp.Key] = kvp.Value;
            }
        }

        var logEntry = new LogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Category = _categoryName,
            Message = message,
            Exception = exception,
            Timestamp = DateTimeOffset.UtcNow,
            Scopes = scopes.AsReadOnly(),
            State = stateDict
        };

        // Obtém o webhook apropriado para este log entry (considera router, múltiplos webhooks e fallback)
        var webhookUrl = _options.GetWebhookForLog(logEntry);

        // Envia de forma assíncrona sem bloquear (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                var (discordMessage, attachment) = _formatter.FormatWithAttachment(logEntry);
                if (discordMessage != null)
                {
                    await _client.SendAsync(discordMessage, attachment, webhookUrl, CancellationToken.None);
                }
            }
            catch
            {
                // Silenciosamente ignora erros de logging para não impactar a aplicação
            }
        });
    }
}
