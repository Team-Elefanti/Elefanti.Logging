using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Discord.Formatting;

/// <summary>
/// Mapeia LogLevels para cores do Discord (valor decimal).
/// </summary>
public static class ColorMapper
{
    private static readonly Dictionary<LogLevel, int> ColorMap = new()
    {
        { LogLevel.Trace, 0x6C757D },       // Cinza
        { LogLevel.Debug, 0x17A2B8 },       // Ciano
        { LogLevel.Information, 0x28A745 }, // Verde
        { LogLevel.Warning, 0xFFC107 },     // Amarelo
        { LogLevel.Error, 0xDC3545 },       // Vermelho
        { LogLevel.Critical, 0x6F1AB6 },    // Roxo escuro
        { LogLevel.None, 0x000000 }         // Preto
    };

    /// <summary>
    /// Obtém a cor correspondente ao LogLevel.
    /// </summary>
    public static int GetColor(LogLevel logLevel)
    {
        return ColorMap.TryGetValue(logLevel, out var color) ? color : 0x000000;
    }
}
