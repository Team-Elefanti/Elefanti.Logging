using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Discord.Formatting;

/// <summary>
/// Mapeia LogLevels para emojis compatíveis com Discord.
/// Usa emojis testados que renderizam corretamente no Discord.
/// </summary>
public static class EmojiMapper
{
    /// <summary>
    /// Emojis padrão compatíveis com Discord por LogLevel.
    /// Estes emojis foram testados e renderizam corretamente em todas as plataformas Discord.
    /// </summary>
    private static readonly Dictionary<LogLevel, string> EmojiMap = new()
    {
        { LogLevel.Trace, "??" },       // Magnifying glass - Trace/Search
        { LogLevel.Debug, "??" },       // Wrench - Debug/Fix
        { LogLevel.Information, "??" }, // Information symbol
        { LogLevel.Warning, "??" },     // Warning sign
        { LogLevel.Error, "?" },       // Cross mark - Error
        { LogLevel.Critical, "??" },    // Police car light - Critical/Alert
        { LogLevel.None, "?" }         // Question mark
    };

    /// <summary>
    /// Obtém o emoji correspondente ao LogLevel.
    /// </summary>
    public static string GetEmoji(LogLevel logLevel)
    {
        return EmojiMap.TryGetValue(logLevel, out var emoji) ? emoji : "?"; // Question mark fallback
    }

    /// <summary>
    /// Obtém o emoji correspondente ao LogLevel com fallback para emojis personalizados.
    /// Se customEmojis for fornecido e contiver o LogLevel, usa o emoji personalizado.
    /// Caso contrário, usa o emoji padrão.
    /// </summary>
    public static string GetEmoji(LogLevel logLevel, Dictionary<LogLevel, string>? customEmojis)
    {
        if (customEmojis != null && customEmojis.TryGetValue(logLevel, out var customEmoji))
        {
            return customEmoji;
        }

        return GetEmoji(logLevel);
    }
}
