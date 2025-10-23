using System.Text;

namespace Elefanti.Logging.Utilities;

/// <summary>
/// Utilitário para formatação de scopes de logging.
/// </summary>
public static class ScopeFormatter
{
    /// <summary>
    /// Formata uma lista de scopes em uma string.
    /// </summary>
    public static string Format(IReadOnlyList<object> scopes, string separator = " ? ")
    {
        if (scopes == null || scopes.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(separator, scopes.Select(FormatScope));
    }

    /// <summary>
    /// Formata scopes em múltiplas linhas.
    /// </summary>
    public static string FormatMultiLine(IReadOnlyList<object> scopes, string indent = "  ")
    {
        if (scopes == null || scopes.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < scopes.Count; i++)
        {
            var scope = scopes[i];
            var currentIndent = new string(' ', i * indent.Length);
            sb.AppendLine($"{currentIndent}{FormatScope(scope)}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formata scopes como uma hierarquia visual.
    /// </summary>
    public static string FormatHierarchy(IReadOnlyList<object> scopes)
    {
        if (scopes == null || scopes.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < scopes.Count; i++)
        {
            var scope = scopes[i];
            var prefix = i == 0 ? "??" : (i == scopes.Count - 1 ? "??" : "??");
            sb.AppendLine($"{prefix} {FormatScope(scope)}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formata um único scope.
    /// </summary>
    public static string FormatScope(object? scope)
    {
        if (scope == null)
        {
            return "null";
        }

        // Se for IEnumerable<KeyValuePair>, formata como propriedades
        if (scope is IEnumerable<KeyValuePair<string, object?>> properties)
        {
            var formatted = string.Join(", ", properties.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return $"{{ {formatted} }}";
        }

        // Se for dictionary, formata como propriedades
        if (scope is System.Collections.IDictionary dictionary)
        {
            var items = new List<string>();
            foreach (var key in dictionary.Keys)
            {
                items.Add($"{key}={dictionary[key]}");
            }
            return $"{{ {string.Join(", ", items)} }}";
        }

        // Default: ToString()
        return scope.ToString() ?? "null";
    }

    /// <summary>
    /// Extrai propriedades estruturadas de um scope.
    /// </summary>
    public static Dictionary<string, object?> ExtractProperties(object? scope)
    {
        var properties = new Dictionary<string, object?>();

        if (scope == null)
        {
            return properties;
        }

        // Se for IEnumerable<KeyValuePair>
        if (scope is IEnumerable<KeyValuePair<string, object?>> kvps)
        {
            foreach (var kvp in kvps)
            {
                properties[kvp.Key] = kvp.Value;
            }
            return properties;
        }

        // Se for Dictionary
        if (scope is System.Collections.IDictionary dictionary)
        {
            foreach (var key in dictionary.Keys)
            {
                properties[key.ToString() ?? "null"] = dictionary[key];
            }
            return properties;
        }

        // Default: adiciona o scope inteiro
        properties["Scope"] = scope;
        return properties;
    }

    /// <summary>
    /// Extrai todas as propriedades de todos os scopes.
    /// </summary>
    public static Dictionary<string, object?> ExtractAllProperties(IReadOnlyList<object> scopes)
    {
        var allProperties = new Dictionary<string, object?>();

        if (scopes == null || scopes.Count == 0)
        {
            return allProperties;
        }

        for (int i = 0; i < scopes.Count; i++)
        {
            var scopeProperties = ExtractProperties(scopes[i]);
            foreach (var kvp in scopeProperties)
            {
                // Adiciona prefixo do índice se houver conflito
                var key = allProperties.ContainsKey(kvp.Key) 
                    ? $"{kvp.Key}_{i}" 
                    : kvp.Key;
                
                allProperties[key] = kvp.Value;
            }
        }

        return allProperties;
    }

    /// <summary>
    /// Conta o número de scopes.
    /// </summary>
    public static int Count(IReadOnlyList<object> scopes)
    {
        return scopes?.Count ?? 0;
    }

    /// <summary>
    /// Verifica se há scopes.
    /// </summary>
    public static bool HasScopes(IReadOnlyList<object>? scopes)
    {
        return scopes != null && scopes.Count > 0;
    }

    /// <summary>
    /// Obtém o último scope (mais interno).
    /// </summary>
    public static object? GetInnermost(IReadOnlyList<object>? scopes)
    {
        if (scopes == null || scopes.Count == 0)
        {
            return null;
        }

        return scopes[scopes.Count - 1];
    }

    /// <summary>
    /// Obtém o primeiro scope (mais externo).
    /// </summary>
    public static object? GetOutermost(IReadOnlyList<object>? scopes)
    {
        if (scopes == null || scopes.Count == 0)
        {
            return null;
        }

        return scopes[0];
    }
}
