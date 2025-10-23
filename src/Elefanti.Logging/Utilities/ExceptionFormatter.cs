using System.Text;

namespace Elefanti.Logging.Utilities;

/// <summary>
/// Utilitário para formatação de exceções.
/// </summary>
public static class ExceptionFormatter
{
    /// <summary>
    /// Formata uma exceção completa incluindo inner exceptions.
    /// </summary>
    public static string Format(Exception exception, bool includeStackTrace = true, int maxDepth = 5)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var sb = new StringBuilder();
        FormatException(sb, exception, 0, maxDepth, includeStackTrace);
        return sb.ToString();
    }

    /// <summary>
    /// Formata apenas a mensagem da exceção sem stack trace.
    /// </summary>
    public static string FormatMessage(Exception exception)
    {
        return Format(exception, includeStackTrace: false);
    }

    /// <summary>
    /// Formata apenas o stack trace da exceção.
    /// </summary>
    public static string FormatStackTrace(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var sb = new StringBuilder();
        FormatStackTraceInternal(sb, exception, 0, 5);
        return sb.ToString();
    }

    /// <summary>
    /// Obtém o tipo completo da exceção (namespace + nome).
    /// </summary>
    public static string GetExceptionType(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return exception.GetType().FullName ?? exception.GetType().Name;
    }

    /// <summary>
    /// Obtém todas as mensagens de exceção (incluindo inner exceptions).
    /// </summary>
    public static IEnumerable<string> GetAllMessages(Exception exception)
    {
        var current = exception;
        var depth = 0;
        const int maxDepth = 10;

        while (current != null && depth < maxDepth)
        {
            yield return current.Message;
            current = current.InnerException;
            depth++;
        }
    }

    /// <summary>
    /// Obtém a exceção mais interna (root cause).
    /// </summary>
    public static Exception GetRootException(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var current = exception;
        var depth = 0;
        const int maxDepth = 10;

        while (current.InnerException != null && depth < maxDepth)
        {
            current = current.InnerException;
            depth++;
        }

        return current;
    }

    /// <summary>
    /// Conta o número de inner exceptions.
    /// </summary>
    public static int CountInnerExceptions(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        int count = 0;
        var current = exception.InnerException;
        const int maxDepth = 10;

        while (current != null && count < maxDepth)
        {
            count++;
            current = current.InnerException;
        }

        return count;
    }

    private static void FormatException(StringBuilder sb, Exception exception, int depth, int maxDepth, bool includeStackTrace)
    {
        if (exception == null || depth >= maxDepth)
        {
            return;
        }

        // Separador para inner exceptions
        if (depth > 0)
        {
            sb.AppendLine();
            sb.AppendLine(new string('=', 80));
            sb.AppendLine($"INNER EXCEPTION #{depth}");
            sb.AppendLine(new string('=', 80));
            sb.AppendLine();
        }

        // Tipo e mensagem
        sb.AppendLine($"Exception Type: {exception.GetType().FullName}");
        sb.AppendLine($"Message: {exception.Message}");
        sb.AppendLine();

        // Source
        if (!string.IsNullOrWhiteSpace(exception.Source))
        {
            sb.AppendLine($"Source: {exception.Source}");
            sb.AppendLine();
        }

        // HResult
        if (exception.HResult != 0)
        {
            sb.AppendLine($"HResult: 0x{exception.HResult:X8}");
            sb.AppendLine();
        }

        // Data
        if (exception.Data.Count > 0)
        {
            sb.AppendLine("Data:");
            foreach (var key in exception.Data.Keys)
            {
                sb.AppendLine($"  {key}: {exception.Data[key]}");
            }
            sb.AppendLine();
        }

        // Stack Trace
        if (includeStackTrace && !string.IsNullOrWhiteSpace(exception.StackTrace))
        {
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(exception.StackTrace);
            sb.AppendLine();
        }

        // Inner Exception (recursivo)
        if (exception.InnerException != null)
        {
            FormatException(sb, exception.InnerException, depth + 1, maxDepth, includeStackTrace);
        }
    }

    private static void FormatStackTraceInternal(StringBuilder sb, Exception exception, int depth, int maxDepth)
    {
        if (exception == null || depth >= maxDepth)
        {
            return;
        }

        if (depth > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"--- Inner Exception #{depth} Stack Trace ---");
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(exception.StackTrace))
        {
            sb.AppendLine(exception.StackTrace);
        }

        if (exception.InnerException != null)
        {
            FormatStackTraceInternal(sb, exception.InnerException, depth + 1, maxDepth);
        }
    }
}
