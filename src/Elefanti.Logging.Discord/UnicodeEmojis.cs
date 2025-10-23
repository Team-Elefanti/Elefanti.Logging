using Microsoft.Extensions.Logging;

namespace Elefanti.Logging.Discord;

/// <summary>
/// Constantes de emojis Unicode compatíveis com Discord e outras plataformas.
/// Estes emojis foram testados e renderizam corretamente em todas as plataformas Discord (desktop, web, mobile)
/// e também funcionam em console, aplicações web, e outros sistemas que suportam Unicode.
/// </summary>
/// <remarks>
/// Use estas constantes para garantir que os emojis sejam exibidos corretamente.
/// Os emojis estão em formato Unicode escape sequence que o C# compila corretamente.
/// </remarks>
public static class UnicodeEmojis
{
    // ========================================
    // Emojis por LogLevel
    // ========================================
    
    /// <summary>?? Police car light - Para logs críticos que requerem atenção imediata</summary>
    public const string Critical = "\U0001F6A8";
    
    /// <summary>? Cross mark - Para erros que impedem operações</summary>
    public const string Error = "\u274C";
    
    /// <summary>?? Warning sign - Para situações não esperadas mas não críticas</summary>
    public const string Warning = "\u26A0\uFE0F";
    
    /// <summary>?? Information - Para informações do fluxo normal da aplicação</summary>
    public const string Information = "\u2139\uFE0F";
    
    /// <summary>?? Wrench - Para informações de debug/desenvolvimento</summary>
    public const string Debug = "\U0001F527";
    
    /// <summary>?? Magnifying glass - Para logs de trace/investigação detalhada</summary>
    public const string Trace = "\U0001F50D";

    // ========================================
    // Emojis para categorias/features
    // ========================================
    
    /// <summary>?? Credit card - Para operações de pagamento</summary>
    public const string Payment = "\U0001F4B3";
    
    /// <summary>?? Closed lock with key - Para operações de autenticação</summary>
    public const string Auth = "\U0001F510";
    
    /// <summary>? Check mark button - Para indicar sucesso</summary>
    public const string Success = "\u2705";
    
    /// <summary>?? E-mail - Para operações relacionadas a email</summary>
    public const string Email = "\U0001F4E7";
    
    /// <summary>?? Key - Para tokens e chaves de acesso</summary>
    public const string Token = "\U0001F511";
    
    /// <summary>?? Paperclip - Para anexos de arquivo</summary>
    public const string Attachment = "\U0001F4CE";
    
    /// <summary>?? Floppy disk - Para operações de banco de dados</summary>
    public const string Database = "\U0001F4BE";
    
    /// <summary>??? Shield - Para operações de segurança</summary>
    public const string Security = "\U0001F6E1\uFE0F";
    
    /// <summary>?? Bust in silhouette - Para operações relacionadas a usuários</summary>
    public const string User = "\U0001F464";
    
    /// <summary>?? Package - Para pedidos/encomendas</summary>
    public const string Order = "\U0001F4E6";

    // ========================================
    // Emojis para UI/Console
    // ========================================
    
    /// <summary>?? Play button - Para indicar início de execução</summary>
    public const string Play = "\u25B6\uFE0F";
    
    /// <summary>?? Stop button - Para indicar parada de execução</summary>
    public const string Stop = "\u23F9\uFE0F";
    
    /// <summary>?? Direct hit - Para indicar alvos ou objetivos</summary>
    public const string Target = "\U0001F3AF";
    
    /// <summary>??? Label - Para tags e etiquetas</summary>
    public const string Tag = "\U0001F3F7\uFE0F";
    
    /// <summary>?? Rocket - Para indicar lançamento ou início rápido</summary>
    public const string Rocket = "\U0001F680";
    
    /// <summary>?? Elephant - Mascote Elefanti</summary>
    public const string Elephant = "\U0001F418";
    
    /// <summary>?? Bar chart - Para gráficos e estatísticas</summary>
    public const string Chart = "\U0001F4CA";
    
    /// <summary>?? Memo - Para documentos e notas</summary>
    public const string Document = "\U0001F4DD";
    
    /// <summary>?? Bookmark - Para marcadores e favoritos</summary>
    public const string Bookmark = "\U0001F516";
    
    /// <summary>?? Light bulb - Para dicas e ideias</summary>
    public const string Lightbulb = "\U0001F4A1";
    
    /// <summary>?? Waving hand - Para saudações e despedidas</summary>
    public const string Wave = "\U0001F44B";
    
    /// <summary>? Rightwards arrow - Para indicar direção ou fluxo</summary>
    public const string Arrow = "\u2192";

    // ========================================
    // Métodos Helper
    // ========================================

    /// <summary>
    /// Retorna um dicionário com os emojis padrão configurados por LogLevel.
    /// Útil para configurar CustomEmojis em DiscordLoggerOptions.
    /// </summary>
    /// <returns>Dicionário mapeando LogLevel para emoji</returns>
    /// <example>
    /// <code>
    /// logging.AddDiscord(options =>
    /// {
    ///     options.CustomEmojis = UnicodeEmojis.GetLogLevelEmojis();
    /// });
    /// </code>
    /// </example>
    public static Dictionary<LogLevel, string> GetLogLevelEmojis()
    {
        return new Dictionary<LogLevel, string>
        {
            [LogLevel.Critical] = Critical,
            [LogLevel.Error] = Error,
            [LogLevel.Warning] = Warning,
            [LogLevel.Information] = Information,
            [LogLevel.Debug] = Debug,
            [LogLevel.Trace] = Trace
        };
    }
}
