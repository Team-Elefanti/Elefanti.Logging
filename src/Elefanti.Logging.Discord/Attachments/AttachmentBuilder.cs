using System.Text;

namespace Elefanti.Logging.Discord.Attachments;

/// <summary>
/// Construtor de anexos para mensagens de log.
/// </summary>
public class AttachmentBuilder
{
    private const int MaxEmbedDescriptionLength = 4096;
    private const int MessageSizeThreshold = 2000;

    /// <summary>
    /// Verifica se uma mensagem precisa ser anexada como arquivo.
    /// </summary>
    public static bool ShouldAttach(string content, int threshold = MessageSizeThreshold)
    {
        return !string.IsNullOrEmpty(content) && content.Length > threshold;
    }

    /// <summary>
    /// Cria um anexo de texto para uma mensagem grande.
    /// </summary>
    public static (string filename, byte[] content) CreateTextAttachment(string text, string prefix = "log")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HHmmss");
        var filename = $"{prefix}-{timestamp}.txt";
        var content = Encoding.UTF8.GetBytes(text);

        return (filename, content);
    }

    /// <summary>
    /// Cria um resumo truncado de um texto longo.
    /// </summary>
    public static string CreateSummary(string fullText, int maxLength = 500)
    {
        if (string.IsNullOrEmpty(fullText) || fullText.Length <= maxLength)
        {
            return fullText;
        }

        var summary = fullText.Substring(0, maxLength);
        var lastNewLine = summary.LastIndexOf('\n');
        
        if (lastNewLine > maxLength / 2)
        {
            summary = summary.Substring(0, lastNewLine);
        }

        return summary.TrimEnd() + "\n\n... (conteúdo completo no arquivo anexo)";
    }

    /// <summary>
    /// Formata um stack trace para melhor legibilidade.
    /// </summary>
    public static string FormatStackTrace(Exception exception)
    {
        var sb = new StringBuilder();
        
        var current = exception;
        var depth = 0;

        while (current != null && depth < 5) // Limita profundidade para evitar loops
        {
            if (depth > 0)
            {
                sb.AppendLine();
                sb.AppendLine(new string('=', 80));
                sb.AppendLine($"INNER EXCEPTION #{depth}");
                sb.AppendLine(new string('=', 80));
                sb.AppendLine();
            }

            sb.AppendLine($"Exception Type: {current.GetType().FullName}");
            sb.AppendLine($"Message: {current.Message}");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(current.Source))
            {
                sb.AppendLine($"Source: {current.Source}");
                sb.AppendLine();
            }

            if (current.Data.Count > 0)
            {
                sb.AppendLine("Data:");
                foreach (var key in current.Data.Keys)
                {
                    sb.AppendLine($"  {key}: {current.Data[key]}");
                }
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(current.StackTrace))
            {
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(current.StackTrace);
            }

            current = current.InnerException;
            depth++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Calcula o tamanho estimado de uma mensagem formatada.
    /// </summary>
    public static int EstimateMessageSize(string? message, Exception? exception, int fieldsCount = 0)
    {
        var size = 0;

        if (!string.IsNullOrEmpty(message))
        {
            size += message.Length;
        }

        if (exception != null)
        {
            size += FormatStackTrace(exception).Length;
        }

        // Adiciona overhead de campos e formatação
        size += fieldsCount * 100; // Estimativa de 100 chars por campo

        return size;
    }
}
