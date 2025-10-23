namespace Elefanti.Logging.Utilities;

/// <summary>
/// Provider de timestamps para logging.
/// </summary>
public static class TimestampProvider
{
    /// <summary>
    /// Obtém o timestamp atual em UTC.
    /// </summary>
    public static DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <summary>
    /// Obtém o timestamp atual em hora local.
    /// </summary>
    public static DateTimeOffset Now => DateTimeOffset.Now;

    /// <summary>
    /// Formata um timestamp em formato ISO 8601.
    /// </summary>
    public static string FormatIso8601(DateTimeOffset timestamp)
    {
        return timestamp.ToString("O"); // ISO 8601: 2025-10-22T15:30:45.1234567Z
    }

    /// <summary>
    /// Formata um timestamp em formato customizado.
    /// </summary>
    public static string Format(DateTimeOffset timestamp, string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            throw new ArgumentException("Formato não pode ser vazio", nameof(format));
        }

        return timestamp.ToString(format);
    }

    /// <summary>
    /// Formata um timestamp para exibição amigável.
    /// </summary>
    public static string FormatFriendly(DateTimeOffset timestamp)
    {
        return timestamp.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// Formata um timestamp para nome de arquivo.
    /// </summary>
    public static string FormatForFilename(DateTimeOffset timestamp)
    {
        return timestamp.ToString("yyyy-MM-dd-HHmmss");
    }

    /// <summary>
    /// Converte timestamp para Unix time (segundos desde 1970-01-01).
    /// </summary>
    public static long ToUnixTimeSeconds(DateTimeOffset timestamp)
    {
        return timestamp.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converte timestamp para Unix time (milissegundos desde 1970-01-01).
    /// </summary>
    public static long ToUnixTimeMilliseconds(DateTimeOffset timestamp)
    {
        return timestamp.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Converte Unix time (segundos) para DateTimeOffset.
    /// </summary>
    public static DateTimeOffset FromUnixTimeSeconds(long seconds)
    {
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }

    /// <summary>
    /// Converte Unix time (milissegundos) para DateTimeOffset.
    /// </summary>
    public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }

    /// <summary>
    /// Calcula o tempo decorrido desde um timestamp.
    /// </summary>
    public static TimeSpan GetElapsedTime(DateTimeOffset since)
    {
        return DateTimeOffset.UtcNow - since;
    }

    /// <summary>
    /// Formata o tempo decorrido de forma amigável.
    /// </summary>
    public static string FormatElapsedTime(DateTimeOffset since)
    {
        var elapsed = GetElapsedTime(since);

        if (elapsed.TotalSeconds < 1)
        {
            return $"{elapsed.TotalMilliseconds:F0}ms";
        }
        if (elapsed.TotalMinutes < 1)
        {
            return $"{elapsed.TotalSeconds:F1}s";
        }
        if (elapsed.TotalHours < 1)
        {
            return $"{elapsed.TotalMinutes:F1}min";
        }
        if (elapsed.TotalDays < 1)
        {
            return $"{elapsed.TotalHours:F1}h";
        }

        return $"{elapsed.TotalDays:F1}d";
    }

    /// <summary>
    /// Verifica se um timestamp é recente (últimos N minutos).
    /// </summary>
    public static bool IsRecent(DateTimeOffset timestamp, int minutes = 5)
    {
        var elapsed = GetElapsedTime(timestamp);
        return elapsed.TotalMinutes <= minutes;
    }

    /// <summary>
    /// Arredonda timestamp para o segundo mais próximo.
    /// </summary>
    public static DateTimeOffset RoundToSecond(DateTimeOffset timestamp)
    {
        var ticks = timestamp.Ticks;
        var roundedTicks = (ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond;
        return new DateTimeOffset(roundedTicks, timestamp.Offset);
    }

    /// <summary>
    /// Arredonda timestamp para o minuto mais próximo.
    /// </summary>
    public static DateTimeOffset RoundToMinute(DateTimeOffset timestamp)
    {
        var ticks = timestamp.Ticks;
        var roundedTicks = (ticks / TimeSpan.TicksPerMinute) * TimeSpan.TicksPerMinute;
        return new DateTimeOffset(roundedTicks, timestamp.Offset);
    }
}
