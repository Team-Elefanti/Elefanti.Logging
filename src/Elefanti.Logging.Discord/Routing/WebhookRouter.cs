using Elefanti.Logging.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Elefanti.Logging.Discord.Routing;

/// <summary>
/// Representa uma regra de roteamento de webhook.
/// </summary>
public class RoutingRule
{
    /// <summary>
    /// Prioridade da regra (maior = mais prioritária).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Predicado que determina se a regra se aplica.
    /// </summary>
    public Func<LogEntry, bool> Predicate { get; set; } = _ => false;

    /// <summary>
    /// URL do webhook para onde enviar se a regra corresponder.
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da regra (para debugging).
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Builder fluente para configuração de roteamento de webhooks.
/// </summary>
public class WebhookRoutingBuilder
{
    private readonly List<RoutingRule> _rules = new();
    private string? _defaultWebhook;
    private int _currentPriority = 100;

    /// <summary>
    /// Roteia logs de uma categoria específica para um webhook.
    /// Suporta wildcards: "MyApp.Services.*"
    /// </summary>
    public WebhookRoutingBuilder RouteCategory(string categoryPattern, string webhookUrl, int? priority = null)
    {
        if (string.IsNullOrWhiteSpace(categoryPattern))
        {
            throw new ArgumentException("Pattern de categoria não pode ser vazio", nameof(categoryPattern));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        // Converte wildcard para regex
        var regexPattern = "^" + Regex.Escape(categoryPattern).Replace("\\*", ".*") + "$";
        var regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = log => regex.IsMatch(log.Category),
            WebhookUrl = webhookUrl,
            Description = $"Category pattern: {categoryPattern}"
        });

        return this;
    }

    /// <summary>
    /// Roteia logs de um LogLevel específico para um webhook.
    /// </summary>
    public WebhookRoutingBuilder RouteLevel(LogLevel logLevel, string webhookUrl, int? priority = null)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = log => log.LogLevel == logLevel,
            WebhookUrl = webhookUrl,
            Description = $"LogLevel: {logLevel}"
        });

        return this;
    }

    /// <summary>
    /// Roteia logs que atendem a uma condição customizada.
    /// </summary>
    public WebhookRoutingBuilder RouteWhen(Func<LogEntry, bool> predicate, string webhookUrl, int? priority = null, string? description = null)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = predicate,
            WebhookUrl = webhookUrl,
            Description = description ?? "Custom predicate"
        });

        return this;
    }

    /// <summary>
    /// Roteia logs que contêm uma exceção de um tipo específico.
    /// </summary>
    public WebhookRoutingBuilder RouteExceptionType<TException>(string webhookUrl, int? priority = null) 
        where TException : Exception
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = log => log.Exception is TException,
            WebhookUrl = webhookUrl,
            Description = $"Exception type: {typeof(TException).Name}"
        });

        return this;
    }

    /// <summary>
    /// Roteia logs que contêm um Event ID específico.
    /// </summary>
    public WebhookRoutingBuilder RouteEventId(int eventId, string webhookUrl, int? priority = null)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = log => log.EventId.Id == eventId,
            WebhookUrl = webhookUrl,
            Description = $"Event ID: {eventId}"
        });

        return this;
    }

    /// <summary>
    /// Roteia logs que contêm um Event ID com um nome específico.
    /// </summary>
    public WebhookRoutingBuilder RouteEventName(string eventName, string webhookUrl, int? priority = null)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name não pode ser vazio", nameof(eventName));
        }

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _rules.Add(new RoutingRule
        {
            Priority = priority ?? _currentPriority++,
            Predicate = log => log.EventId.Name == eventName,
            WebhookUrl = webhookUrl,
            Description = $"Event name: {eventName}"
        });

        return this;
    }

    /// <summary>
    /// Define o webhook padrão (fallback) quando nenhuma regra corresponde.
    /// </summary>
    public WebhookRoutingBuilder RouteDefault(string webhookUrl)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Webhook URL não pode ser vazia", nameof(webhookUrl));
        }

        _defaultWebhook = webhookUrl;
        return this;
    }

    /// <summary>
    /// Constrói o router com as regras configuradas.
    /// </summary>
    public WebhookRouter Build()
    {
        return new WebhookRouter(_rules.OrderByDescending(r => r.Priority).ToList(), _defaultWebhook);
    }

    /// <summary>
    /// Valida as configurações de roteamento.
    /// </summary>
    public void Validate()
    {
        if (!_rules.Any() && string.IsNullOrWhiteSpace(_defaultWebhook))
        {
            throw new InvalidOperationException(
                "Pelo menos uma regra de roteamento ou um webhook padrão deve ser configurado.");
        }

        foreach (var rule in _rules)
        {
            if (!Uri.TryCreate(rule.WebhookUrl, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Webhook URL inválida na regra '{rule.Description}': {rule.WebhookUrl}");
            }

            if (!uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.EndsWith("discordapp.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Webhook deve ser do Discord na regra '{rule.Description}': {rule.WebhookUrl}");
            }
        }

        if (!string.IsNullOrWhiteSpace(_defaultWebhook))
        {
            if (!Uri.TryCreate(_defaultWebhook, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Webhook URL padrão inválida: {_defaultWebhook}");
            }

            if (!uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.EndsWith("discordapp.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Webhook padrão deve ser do Discord: {_defaultWebhook}");
            }
        }
    }
}

/// <summary>
/// Router de webhooks baseado em regras.
/// </summary>
public class WebhookRouter
{
    private readonly IReadOnlyList<RoutingRule> _rules;
    private readonly string? _defaultWebhook;

    internal WebhookRouter(IReadOnlyList<RoutingRule> rules, string? defaultWebhook)
    {
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        _defaultWebhook = defaultWebhook;
    }

    /// <summary>
    /// Determina o webhook apropriado para um log entry.
    /// </summary>
    public string? Route(LogEntry logEntry)
    {
        if (logEntry == null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        // Procura a primeira regra que corresponde (já ordenada por prioridade)
        foreach (var rule in _rules)
        {
            try
            {
                if (rule.Predicate(logEntry))
                {
                    return rule.WebhookUrl;
                }
            }
            catch
            {
                // Ignora erros em predicados e continua para próxima regra
                continue;
            }
        }

        // Retorna webhook padrão se nenhuma regra corresponder
        return _defaultWebhook;
    }

    /// <summary>
    /// Obtém informações sobre as regras configuradas (para debugging).
    /// </summary>
    public IReadOnlyList<(int Priority, string Description, string WebhookUrl)> GetRulesInfo()
    {
        return _rules
            .Select(r => (r.Priority, r.Description ?? "No description", r.WebhookUrl))
            .ToList();
    }
}
