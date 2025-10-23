# 🐘 Elefanti.Logging

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Elefanti.Logging** é um sistema de logging extensível integrado ao `Microsoft.Extensions.Logging`, permitindo enviar logs para diversos destinos externos de forma simples e eficiente.

---

## 🚀 Início Rápido

### Instalação

```bash
# Core (abstrações)
dotnet add package Elefanti.Logging

# Discord Provider
dotnet add package Elefanti.Logging.Discord
```

> **Nota:** Os pacotes ainda não foram publicados no NuGet. Para usar o projeto, clone o repositório e adicione as referências locais.

### Uso Básico

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elefanti.Logging.Discord.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Configurar logging com Discord
builder.Services.AddLogging(logging =>
{
    logging.AddDiscord(options =>
    {
        options.WebhookUrl = "https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_TOKEN";
        options.Username = "MyApp Logger";
        options.MinimumLevel = LogLevel.Information;
    });
});

// Registrar serviços
builder.Services.AddSingleton<MyService>();

var host = builder.Build();
var service = host.Services.GetRequiredService<MyService>();
service.DoWork();

public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.LogInformation("Hello from {ServiceName}!", nameof(MyService));
        _logger.LogWarning("This is a warning message");
        _logger.LogError("This is an error message");
    }
}
```

---

## 📦 Providers Disponíveis

| Provider | Status | Descrição |
|----------|--------|-----------|
| **Discord** | ✅ Disponível | Envia logs para canais do Discord via Webhooks |
| **Slack** | 🚧 Planejado | Envia logs para canais do Slack |
| **Telegram** | 🚧 Planejado | Envia logs para grupos/canais do Telegram |
| **Teams** | 🚧 Planejado | Envia logs para canais do Microsoft Teams |

---

## 🎮 Discord Provider

### Recursos Implementados

- ✅ **Rich Embeds** - Mensagens formatadas com cores e emojis Unicode
- ✅ **Structured Logging** - Suporte completo a parâmetros estruturados
- ✅ **Event IDs** - Rastreamento de eventos específicos
- ✅ **Scopes** - Contexto hierárquico de logging
- ✅ **Async/Non-blocking** - Não bloqueia a thread principal
- ✅ **Customizável** - Username, avatar, emojis e cores personalizáveis
- ✅ **Múltiplos Webhooks** - Webhooks diferentes por LogLevel
- ✅ **Routing Inteligente** - Roteamento baseado em regras customizadas
- ✅ **Routing por Atributos** - Atributo `[DiscordWebhook]` em classes/métodos
- ✅ **Anexos de Arquivo** - Stack traces grandes enviadas como arquivo `.txt`
- ✅ **Unicode Emojis** - Biblioteca completa de emojis compatíveis com Discord

### Esquema de Cores

| LogLevel | Cor (Hex) | Emoji Padrão |
|----------|-----------|--------------|
| Trace | `#6C757D` (Cinza) | 🔍 |
| Debug | `#17A2B8` (Ciano) | 🔧 |
| Information | `#28A745` (Verde) | ℹ️ |
| Warning | `#FFC107` (Amarelo) | ⚠️ |
| Error | `#DC3545` (Vermelho) | ❌ |
| Critical | `#6F1AB6` (Roxo) | 🚨 |

### Configuração Completa

```csharp
logging.AddDiscord(options =>
{
    // Obrigatório
    options.WebhookUrl = "https://discord.com/api/webhooks/...";
    
    // Customização visual
    options.Username = "🐘 Elefanti Logger";
    options.AvatarUrl = "https://example.com/avatar.png";
    
    // Filtros
    options.MinimumLevel = LogLevel.Information;
    
    // Formatação
    options.IncludeTimestamp = true;
    options.IncludeCategory = true;
    options.IncludeEventId = true;
    options.IncludeScopes = true;
    
    // Retry e Timeout
    options.MaxRetryAttempts = 3;
    options.TimeoutSeconds = 30;
    
    // Anexos de arquivo
    options.EnableAttachments = true;
    options.AttachmentSizeThreshold = 2000;
    options.AlwaysAttachStackTraces = false;
    
    // Emojis personalizados
    options.CustomEmojis = new Dictionary<LogLevel, string>
    {
        [LogLevel.Critical] = "🚨",
        [LogLevel.Error] = "❌",
        [LogLevel.Warning] = "⚠️",
        [LogLevel.Information] = "ℹ️"
    };
});
```

### Overloads Disponíveis

```csharp
// 1. Básico - apenas URL
logging.AddDiscord("https://discord.com/api/webhooks/...");

// 2. Com username
logging.AddDiscord("https://discord.com/...", "MyApp Logger");

// 3. Com username e avatar
logging.AddDiscord("https://discord.com/...", "MyApp", "https://example.com/avatar.png");

// 4. Configuração completa
logging.AddDiscord(options => { /* ... */ });
```

### Múltiplos Webhooks por LogLevel

Envie logs de diferentes níveis para canais diferentes:

```csharp
logging.AddDiscord(options =>
{
    options.Username = "🐘 Multi-Webhook Logger";
    
    options.ConfigureMultipleWebhooks(webhooks =>
    {
        webhooks.SetWebhook(LogLevel.Critical, "https://discord.com/.../critical");
        webhooks.SetWebhook(LogLevel.Error, "https://discord.com/.../errors");
        webhooks.SetWebhook(LogLevel.Warning, "https://discord.com/.../warnings");
        webhooks.SetWebhook(LogLevel.Information, "https://discord.com/.../info");
        webhooks.DefaultWebhook = "https://discord.com/.../default";
    });
});
```

### Routing Inteligente

Roteie logs baseado em regras customizadas:

```csharp
logging.AddDiscord(options =>
{
    options.Username = "🎯 Smart Router";
    
    options.ConfigureRouting(routing =>
    {
        // Prioridade: logs críticos
        routing.RouteLevel(LogLevel.Critical, "https://discord.com/.../critical", priority: 1000);
        
        // Por categoria (wildcards)
        routing.RouteCategory("*.Payment.*", "https://discord.com/.../payments");
        routing.RouteCategory("*.Auth.*", "https://discord.com/.../security");
        
        // Por tipo de exceção
        routing.RouteExceptionType<SqlException>("https://discord.com/.../database");
        
        // Por Event ID
        routing.RouteEventId(5000, "https://discord.com/.../critical");
        routing.RouteEventName("SecurityAlert", "https://discord.com/.../security");
        
        // Condicional customizado
        routing.RouteWhen(
            log => log.Message.Contains("SQL"),
            "https://discord.com/.../database",
            description: "Database logs"
        );
        
        // Fallback
        routing.RouteDefault("https://discord.com/.../default");
    });
});
```

### Routing por Atributos

Use o atributo `[DiscordWebhook]` em classes para definir webhooks específicos:

```csharp
[DiscordWebhook("https://discord.com/api/webhooks/.../payments")]
public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    
    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }
    
    public void ProcessPayment()
    {
        _logger.LogInformation("Processing payment");
        // Logs vão automaticamente para o webhook de payments
    }
}
```

**Configuração:**
```csharp
logging.AddDiscord(options =>
{
    options.UseAttributeRouting("https://discord.com/.../default");
    // Habilita roteamento por atributos com fallback
});
```

### Anexos de Arquivo

Stack traces longas são automaticamente enviadas como arquivo:

```csharp
logging.AddDiscord(options =>
{
    options.EnableAttachments = true;
    options.AttachmentSizeThreshold = 2000; // caracteres
    options.AlwaysAttachStackTraces = false; // opcional: sempre anexa exceptions
});
```

**Resultado:**
- Mensagens < 2000 chars → Embed normal
- Mensagens > 2000 chars → Embed resumido + arquivo `.txt` anexo
- Stack traces podem ser sempre anexadas quando `AlwaysAttachStackTraces = true`

---

## 📖 Exemplos

### Structured Logging

```csharp
_logger.LogInformation(
    "Usuário {UserId} realizou uma compra de {Amount:C}",
    12345, 150.50m);
```

**Resultado no Discord:**
```
ℹ️ Information
Usuário 12345 realizou uma compra de R$ 150,50

📦 Category: MyApp.Services.PaymentService
📊 Data:
• UserId: 12345
• Amount: 150.50
```

### Event IDs

```csharp
_logger.LogInformation(
    new EventId(1001, "UserLogin"),
    "Login bem-sucedido para {Username}",
    "john.doe");
```

### Scopes

```csharp
using (_logger.BeginScope("Pedido #{OrderId}", 9876))
{
    _logger.LogInformation("Validando pedido...");
    _logger.LogInformation("Processando pagamento...");
    _logger.LogInformation("Pedido concluído!");
}
```

### Exceções

```csharp
try
{
    // código que pode falhar
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar pagamento");
}
```

---

## 🎯 Projeto de Exemplo

Confira o projeto de exemplo completo em [`ConsoleLoggingSample/`](./ConsoleLoggingSample/):

```bash
cd ConsoleLoggingSample
dotnet run
```

O exemplo inclui **5 demonstrações interativas**:

### 1️⃣ Exemplo Básico
- ✅ Configuração com DI
- ✅ Todos os níveis de log
- ✅ Structured logging
- ✅ Event IDs
- ✅ Scopes
- ✅ Tratamento de exceções

### 2️⃣ Múltiplos Webhooks
- ✅ Webhooks diferentes por LogLevel
- ✅ Canais específicos (Critical, Errors, Warnings, Info)
- ✅ Sistema de fallback

### 3️⃣ Routing Avançado
- ✅ Routing por categoria (wildcards)
- ✅ Routing por tipo de exceção
- ✅ Routing por Event ID/Name
- ✅ Routing condicional customizado
- ✅ Sistema de prioridades

### 4️⃣ Anexos de Arquivo
- ✅ Stack traces grandes como arquivo .txt
- ✅ Detecção automática
- ✅ Resumo inteligente no embed
- ✅ Suporte a Inner Exceptions

### 5️⃣ Attribute Routing
- ✅ Roteamento por atributos `[DiscordWebhook]`
- ✅ Aplicável em classes e métodos
- ✅ Cache de atributos via Reflection
- ✅ Sistema de fallback

**Menu Interativo:** Escolha qual exemplo executar ou execute todos em sequência!

---

## 🏗️ Arquitetura

```
Elefanti.Logging/
├── src/
│   ├── Elefanti.Logging/              # Core - Abstrações
│   │   ├── Abstractions/
│   │   │   ├── IExternalLoggerProvider.cs
│   │   │   ├── ILogMessageFormatter.cs
│   │   │   ├── ILogTransport.cs
│   │   │   └── ILogFilter.cs
│   │   ├── Configuration/
│   │   │   └── ExternalLoggerOptions.cs
│   │   ├── Extensions/
│   │   │   └── LoggingBuilderExtensions.cs
│   │   ├── Models/
│   │   │   └── LogEntry.cs
│   │   └── Utilities/
│   │       ├── ExceptionFormatter.cs
│   │       ├── LogLevelHelper.cs
│   │       ├── ScopeFormatter.cs
│   │       └── TimestampProvider.cs
│   │
│   └── Elefanti.Logging.Discord/      # Discord Provider
│       ├── Attachments/
│       │   └── AttachmentBuilder.cs
│       ├── Attributes/
│       │   └── DiscordWebhookAttribute.cs
│       ├── Client/
│       │   └── DiscordWebhookClient.cs
│       ├── Configuration/
│       │   ├── DiscordLoggerOptions.cs
│       │   ├── DiscordOptionsValidator.cs
│       │   └── WebhookConfiguration.cs
│       ├── Extensions/
│       │   └── DiscordLoggingExtensions.cs
│       ├── Formatting/
│       │   ├── ColorMapper.cs
│       │   ├── DiscordMessageFormatter.cs
│       │   └── EmojiMapper.cs
│       ├── Models/
│       │   └── DiscordModels.cs
│       ├── Providers/
│       │   └── DiscordLoggerProvider.cs
│       ├── Routing/
│       │   ├── AttributeBasedRouter.cs
│       │   └── WebhookRouter.cs
│       └── UnicodeEmojis.cs
│
├── samples/
│   └── ConsoleLoggingSample/          # Projeto de exemplo
│       ├── Examples/
│       │   ├── AdvancedRoutingExample.cs
│       │   ├── AttributeRoutingExample.cs
│       │   ├── BasicExample.cs
│       │   ├── FileAttachmentsExample.cs
│       │   └── MultipleWebhooksExample.cs
│       ├── Helpers/
│       │   └── ConfigurationHelper.cs
│       ├── Services/
│       │   ├── AdvancedRoutingApp.cs
│       │   ├── AuthServiceWithAttribute.cs
│       │   ├── BasicExampleApp.cs
│       │   ├── FileAttachmentsApp.cs
│       │   ├── MultipleWebhooksApp.cs
│       │   └── PaymentServiceWithAttribute.cs
│       └── Program.cs
│
└── tests/                             # (planejado)
    ├── Elefanti.Logging.Tests/
    └── Elefanti.Logging.Discord.Tests/
```

---

## 🔧 Requisitos

- **.NET 9.0** ou superior
- **Microsoft.Extensions.Logging** 9.0.0+
- **Microsoft.Extensions.Logging.Abstractions** 9.0.0+
- **Microsoft.Extensions.Http** 9.0.0+ (Discord Provider)
- **Microsoft.Extensions.DependencyInjection** 9.0.0+ (Discord Provider)

---

## 🎨 Unicode Emojis

O projeto inclui uma biblioteca completa de emojis Unicode testados e compatíveis com Discord:

```csharp
using Elefanti.Logging.Discord;

// Usar emojis predefinidos
var emojis = UnicodeEmojis.GetLogLevelEmojis();

// Emojis individuais
Console.WriteLine(UnicodeEmojis.Rocket);    // 🚀
Console.WriteLine(UnicodeEmojis.Elephant);  // 🐘
Console.WriteLine(UnicodeEmojis.Success);   // ✅
Console.WriteLine(UnicodeEmojis.Error);     // ❌
```

**Categorias disponíveis:**
- **LogLevel Emojis**: Critical, Error, Warning, Information, Debug, Trace
- **Feature Emojis**: Payment, Auth, Security, Database, Email, Token
- **UI Emojis**: Rocket, Target, Chart, Document, Lightbulb, Wave

---

## 📝 Roadmap

Confira o [ROADMAP.md](./ROADMAP.md) para detalhes completos sobre:
- ✅ Features implementadas
- 🚧 Features em desenvolvimento
- 📝 Features planejadas
- ⏸️ Ideias futuras

### Status Atual (Fase 1 & 2)

**✅ Implementado:**
- Core abstractions e interfaces
- Utilitários compartilhados (ExceptionFormatter, LogLevelHelper, etc.)
- Discord provider completo
- Discord webhook client com HTTP
- Message formatter com rich embeds
- Múltiplos webhooks por LogLevel
- Routing builder pattern completo
- Routing por atributos
- Anexos de arquivo para mensagens grandes
- Unicode emojis library
- 5 exemplos interativos funcionais

**📝 Próximos Passos:**
- [ ] Testes unitários (Core + Discord)
- [ ] Testes de integração
- [ ] Rate limiting avançado
- [ ] Retry policy com Polly
- [ ] Publicação no NuGet
- [ ] Documentação XML completa
- [ ] CI/CD pipeline
- [ ] Providers para Slack, Telegram e Teams

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Fork o repositório
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

## 🐘 Sobre

Desenvolvido com ❤️ pela **Team Elefanti**

---

**Nota:** Este é um projeto em desenvolvimento ativo. APIs podem mudar até a versão 1.0.0.
