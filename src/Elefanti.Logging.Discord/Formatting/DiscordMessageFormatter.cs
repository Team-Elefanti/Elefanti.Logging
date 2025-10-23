using Elefanti.Logging.Abstractions;
using Elefanti.Logging.Discord.Attachments;
using Elefanti.Logging.Discord.Configuration;
using Elefanti.Logging.Discord.Models;
using Elefanti.Logging.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Elefanti.Logging.Discord.Formatting;

/// <summary>
/// Formatador de mensagens para Discord embeds.
/// </summary>
public class DiscordMessageFormatter : ILogMessageFormatter
{
    private readonly DiscordLoggerOptions _options;
    private const int MaxDescriptionLength = 4096;
    private const int MaxFieldValueLength = 1024;
    private const int MaxFieldNameLength = 256;

    public DiscordMessageFormatter(DiscordLoggerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Formata um log entry e retorna a mensagem formatada e um anexo opcional.
    /// </summary>
    public (DiscordWebhookMessage message, (string filename, byte[] content)? attachment) FormatWithAttachment(LogEntry logEntry)
    {
        if (logEntry == null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        (string filename, byte[] content)? attachment = null;
        string? exceptionText = null;
        string? displayExceptionText = null;

        // Processa exceção se existir
        if (logEntry.Exception != null)
        {
            exceptionText = AttachmentBuilder.FormatStackTrace(logEntry.Exception);
            
            // Decide se deve anexar a stack trace como arquivo
            if (_options.EnableAttachments &&
                (_options.AlwaysAttachStackTraces || AttachmentBuilder.ShouldAttach(exceptionText, _options.AttachmentSizeThreshold)))
            {
                attachment = AttachmentBuilder.CreateTextAttachment(exceptionText, "stacktrace");
                displayExceptionText = AttachmentBuilder.CreateSummary(exceptionText, 500);
            }
            else
            {
                displayExceptionText = exceptionText;
            }
        }

        var embed = new DiscordEmbed
        {
            Color = ColorMapper.GetColor(logEntry.LogLevel),
            Timestamp = _options.IncludeTimestamp ? logEntry.Timestamp : null,
            Fields = new List<EmbedField>()
        };

        // Título com emoji e nível
        var emoji = EmojiMapper.GetEmoji(logEntry.LogLevel, _options.CustomEmojis);
        embed.Title = $"{emoji} {logEntry.LogLevel}";

        // Descrição (mensagem principal)
        var descriptionBuilder = new StringBuilder();
        
        if (!string.IsNullOrWhiteSpace(logEntry.Message))
        {
            descriptionBuilder.AppendLine($"**{TruncateText(logEntry.Message, MaxDescriptionLength - 200)}**");
            descriptionBuilder.AppendLine();
        }

        // Exception
        if (!string.IsNullOrWhiteSpace(displayExceptionText))
        {
            descriptionBuilder.AppendLine("```");
            descriptionBuilder.AppendLine(TruncateText(displayExceptionText, MaxDescriptionLength - descriptionBuilder.Length - 100));
            descriptionBuilder.AppendLine("```");

            // Se anexou arquivo, adiciona indicador
            if (attachment.HasValue)
            {
                descriptionBuilder.AppendLine();
                descriptionBuilder.AppendLine($"{UnicodeEmojis.Document} **Stack trace completo disponível no arquivo anexado**");
            }
        }

        embed.Description = descriptionBuilder.ToString();

        // Category
        if (_options.IncludeCategory && !string.IsNullOrWhiteSpace(logEntry.Category))
        {
            embed.Fields.Add(new EmbedField
            {
                Name = $"{UnicodeEmojis.Order} Category",
                Value = $"`{TruncateText(logEntry.Category, MaxFieldValueLength)}`",
                Inline = true
            });
        }

        // Event ID
        if (_options.IncludeEventId && logEntry.EventId.Id != 0)
        {
            embed.Fields.Add(new EmbedField
            {
                Name = $"{UnicodeEmojis.Bookmark} Event ID",
                Value = $"`{logEntry.EventId.Id}`" + (string.IsNullOrWhiteSpace(logEntry.EventId.Name) ? "" : $" ({logEntry.EventId.Name})"),
                Inline = true
            });
        }

        // Scopes
        if (_options.IncludeScopes && logEntry.Scopes.Any())
        {
            var scopesText = string.Join($" {UnicodeEmojis.Arrow} ", logEntry.Scopes.Select(s => s?.ToString() ?? "null"));
            embed.Fields.Add(new EmbedField
            {
                Name = $"{UnicodeEmojis.Trace} Scopes",
                Value = $"`{TruncateText(scopesText, MaxFieldValueLength)}`",
                Inline = false
            });
        }

        // State (parâmetros estruturados)
        if (logEntry.State.Any())
        {
            var stateBuilder = new StringBuilder();
            foreach (var kvp in logEntry.State.Take(10)) // Limita a 10 itens
            {
                if (kvp.Key != "{OriginalFormat}")
                {
                    stateBuilder.AppendLine($"• **{kvp.Key}**: `{kvp.Value}`");
                }
            }

            if (stateBuilder.Length > 0)
            {
                embed.Fields.Add(new EmbedField
                {
                    Name = $"{UnicodeEmojis.Chart} Data",
                    Value = TruncateText(stateBuilder.ToString(), MaxFieldValueLength),
                    Inline = false
                });
            }
        }

        // Footer
        embed.Footer = new EmbedFooter
        {
            Text = $"Elefanti.Logging • {Environment.MachineName}"
        };

        var message = new DiscordWebhookMessage
        {
            Username = _options.Username,
            AvatarUrl = _options.AvatarUrl,
            Embeds = new List<DiscordEmbed> { embed }
        };

        return (message, attachment);
    }

    public object Format(LogEntry logEntry)
    {
        return FormatWithAttachment(logEntry).message;
    }

    private static string FormatException(Exception exception)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{exception.GetType().Name}: {exception.Message}");
        
        if (!string.IsNullOrWhiteSpace(exception.StackTrace))
        {
            sb.AppendLine();
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(exception.StackTrace);
        }

        if (exception.InnerException != null)
        {
            sb.AppendLine();
            sb.AppendLine("Inner Exception:");
            sb.AppendLine(FormatException(exception.InnerException));
        }

        return sb.ToString();
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text;
        }

        return text.Substring(0, maxLength - 3) + "...";
    }
}
