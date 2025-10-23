using System.Collections.Concurrent;
using System.Reflection;
using Elefanti.Logging.Discord.Attributes;
using Elefanti.Logging.Models;

namespace Elefanti.Logging.Discord.Routing;

/// <summary>
/// Router que utiliza atributos [DiscordWebhook] para determinar o webhook.
/// </summary>
public class AttributeBasedRouter
{
    private readonly ConcurrentDictionary<string, WebhookAttributeInfo?> _cache = new();
    private readonly string? _defaultWebhook;

    public AttributeBasedRouter(string? defaultWebhook = null)
    {
        _defaultWebhook = defaultWebhook;
    }

    /// <summary>
    /// Obtém o webhook baseado em atributos da categoria do log.
    /// </summary>
    public string? Route(LogEntry logEntry)
    {
        if (logEntry == null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        if (string.IsNullOrWhiteSpace(logEntry.Category))
        {
            return _defaultWebhook;
        }

        // Busca no cache
        var webhookInfo = _cache.GetOrAdd(logEntry.Category, category =>
        {
            return FindWebhookAttribute(category);
        });

        return webhookInfo?.WebhookUrl ?? _defaultWebhook;
    }

    /// <summary>
    /// Limpa o cache de atributos.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Obtém informações sobre o cache.
    /// </summary>
    public (int Count, IEnumerable<string> Categories) GetCacheInfo()
    {
        return (_cache.Count, _cache.Keys);
    }

    private WebhookAttributeInfo? FindWebhookAttribute(string category)
    {
        try
        {
            // Tenta encontrar o tipo pela categoria (geralmente é o FullName do tipo)
            var type = FindTypeByCategory(category);
            if (type == null)
            {
                return null;
            }

            // Busca atributo no tipo
            var attribute = type.GetCustomAttribute<DiscordWebhookAttribute>(inherit: true);
            if (attribute != null)
            {
                return new WebhookAttributeInfo
                {
                    WebhookUrl = attribute.WebhookUrl,
                    Name = attribute.Name,
                    Priority = attribute.Priority,
                    AllowFallback = attribute.AllowFallback,
                    SourceType = type
                };
            }

            return null;
        }
        catch
        {
            // Em caso de erro (tipo não encontrado, etc.), retorna null
            return null;
        }
    }

    private Type? FindTypeByCategory(string category)
    {
        // A categoria geralmente é o nome completo do tipo (namespace + nome)
        // Ex: "MyApp.Services.PaymentService"

        try
        {
            // Tenta encontrar o tipo em todos os assemblies carregados
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(category);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch
                {
                    // Ignora erros de assembly (pode não ter permissão, etc.)
                    continue;
                }
            }

            // Se não encontrou o tipo exato, tenta encontrar por nome simples
            // Ex: se a categoria for "PaymentService", procura qualquer tipo com esse nome
            var simpleName = category.Contains('.') 
                ? category.Substring(category.LastIndexOf('.') + 1) 
                : category;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t.Name == simpleName || t.FullName == category);

                    foreach (var type in types)
                    {
                        // Verifica se tem o atributo
                        if (type.GetCustomAttribute<DiscordWebhookAttribute>(inherit: true) != null)
                        {
                            return type;
                        }
                    }
                }
                catch
                {
                    // Ignora erros
                    continue;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Informações sobre um webhook encontrado via atributo.
/// </summary>
internal class WebhookAttributeInfo
{
    public required string WebhookUrl { get; init; }
    public string? Name { get; init; }
    public int Priority { get; init; }
    public bool AllowFallback { get; init; }
    public Type? SourceType { get; init; }
}
