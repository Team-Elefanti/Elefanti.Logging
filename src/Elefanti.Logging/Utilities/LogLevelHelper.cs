using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Utilities;

/// <summary>
/// Utilitário para conversões e operações com LogLevel.
/// </summary>
public static class LogLevelHelper
{
    /// <summary>
    /// Converte LogLevel para string descritiva.
    /// </summary>
    public static string ToString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            LogLevel.None => "None",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Converte string para LogLevel.
    /// </summary>
    public static LogLevel Parse(string logLevel)
    {
        return logLevel?.ToLowerInvariant() switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "information" or "info" => LogLevel.Information,
            "warning" or "warn" => LogLevel.Warning,
            "error" => LogLevel.Error,
            "critical" or "crit" => LogLevel.Critical,
            "none" => LogLevel.None,
            _ => throw new ArgumentException($"LogLevel inválido: {logLevel}", nameof(logLevel))
        };
    }

    /// <summary>
    /// Tenta converter string para LogLevel.
    /// </summary>
    public static bool TryParse(string? logLevel, out LogLevel result)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                result = LogLevel.None;
                return false;
            }

            result = Parse(logLevel);
            return true;
        }
        catch
        {
            result = LogLevel.None;
            return false;
        }
    }

    /// <summary>
    /// Verifica se um LogLevel é considerado crítico (Error ou Critical).
    /// </summary>
    public static bool IsCritical(LogLevel logLevel)
    {
        return logLevel is LogLevel.Error or LogLevel.Critical;
    }

    /// <summary>
    /// Verifica se um LogLevel deve ser enviado dado um nível mínimo.
    /// </summary>
    public static bool ShouldLog(LogLevel logLevel, LogLevel minimumLevel)
    {
        if (logLevel == LogLevel.None)
        {
            return false;
        }

        return logLevel >= minimumLevel;
    }

    /// <summary>
    /// Obtém o valor de severidade numérico do LogLevel (0-6).
    /// </summary>
    public static int GetSeverity(LogLevel logLevel)
    {
        return (int)logLevel;
    }

    /// <summary>
    /// Compara dois LogLevels e retorna o mais severo.
    /// </summary>
    public static LogLevel GetMoreSevere(LogLevel level1, LogLevel level2)
    {
        return level1 > level2 ? level1 : level2;
    }

    /// <summary>
    /// Compara dois LogLevels e retorna o menos severo.
    /// </summary>
    public static LogLevel GetLessSevere(LogLevel level1, LogLevel level2)
    {
        return level1 < level2 ? level1 : level2;
    }

    /// <summary>
    /// Obtém todos os LogLevels válidos (exceto None).
    /// </summary>
    public static IEnumerable<LogLevel> GetAllLevels()
    {
        yield return LogLevel.Trace;
        yield return LogLevel.Debug;
        yield return LogLevel.Information;
        yield return LogLevel.Warning;
        yield return LogLevel.Error;
        yield return LogLevel.Critical;
    }

    /// <summary>
    /// Verifica se o LogLevel está no range válido.
    /// </summary>
    public static bool IsValid(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Trace && logLevel <= LogLevel.None;
    }
}
