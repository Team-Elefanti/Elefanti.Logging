# 🗺️ Roadmap - Elefanti.Logging

Este documento descreve o planejamento de implementação do **Elefanti.Logging**, um sistema de logging extensível integrado ao `Microsoft.Extensions.Logging`, com foco inicial no provider **Discord**.

---

## 📋 Visão Geral

### Status do Projeto
- ✅ **Concluído**
- 🚧 **Em Desenvolvimento**
- 📝 **Planejado**
- ⏸️ **Pausado/Futuro**

---

## 🎯 Fase 1: Core (Elefanti.Logging) - 📝 Planejado

### 1.1 Abstrações e Interfaces Base
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `IExternalLoggerProvider` - Interface base para provedores externos
- [ ] `ILogMessageFormatter` - Interface para formatação de mensagens
- [ ] `ILogTransport` - Interface para transporte de logs
- [ ] `ILogFilter` - Interface para filtragem de logs
- [ ] `ExternalLoggerProviderBase` - Classe base abstrata com lógica comum

**Entregáveis:**
```
src/Elefanti.Logging/
├── Abstractions/
│   ├── IExternalLoggerProvider.cs
│   ├── ILogMessageFormatter.cs
│   ├── ILogTransport.cs
│   └── ILogFilter.cs
└── Providers/
    └── ExternalLoggerProviderBase.cs
```

---

### 1.2 Integração com Microsoft.Extensions.Logging
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `ExternalLogger` - Implementação de `ILogger`
- [ ] Suporte completo a `LogLevel`
- [ ] Suporte a scopes (`ILogger.BeginScope`)
- [ ] Suporte a structured logging (parâmetros)
- [ ] Event IDs

**Entregáveis:**
```
src/Elefanti.Logging/
├── Logging/
│   ├── ExternalLogger.cs
│   ├── LogEntry.cs
│   └── LogScope.cs
└── Models/
    └── StructuredLogData.cs
```

---

### 1.3 Extension Methods e Configuração Fluente
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `ILoggingBuilder` extensions
- [ ] API fluente para configuração
- [ ] Suporte a múltiplos provedores
- [ ] Configuração via `appsettings.json`
- [ ] Validação de configurações

**Entregáveis:**
```
src/Elefanti.Logging/
├── Extensions/
│   ├── LoggingBuilderExtensions.cs
│   └── ServiceCollectionExtensions.cs
└── Configuration/
    ├── ExternalLoggerOptions.cs
    └── ConfigurationValidator.cs
```

**Exemplo de Uso:**
```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddElefantiLogging(options =>
    {
        options.MinimumLevel = LogLevel.Information;
        options.IncludeScopes = true;
    });
});
```

---

### 1.4 Utilitários Compartilhados
**Prioridade:** Média | **Status:** ✅ Concluído

**Componentes:**
- [x] `LogLevelHelper` - Conversões e mapeamentos de LogLevel
- [x] `ExceptionFormatter` - Formatação de exceptions e stack traces
- [x] `ScopeFormatter` - Formatação de scopes
- [x] `TimestampProvider` - Provider de timestamps (UTC)

**Entregáveis:**
```
src/Elefanti.Logging/
└── Utilities/
    ├── LogLevelHelper.cs ✅
    ├── ExceptionFormatter.cs ✅
    ├── ScopeFormatter.cs ✅
    └── TimestampProvider.cs ✅
```

---

## 🎮 Fase 2: Discord Provider (Elefanti.Logging.Discord) - 📝 Planejado

### 2.1 Implementação Base do Provider
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `DiscordLoggerProvider` - Implementa `ILoggerProvider` e interfaces core
- [ ] `DiscordLogger` - Logger específico do Discord
- [ ] `DiscordLoggerOptions` - Configurações do provider
- [ ] Validação de configurações do Discord

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Providers/
│   └── DiscordLoggerProvider.cs
├── Logging/
│   └── DiscordLogger.cs
└── Configuration/
    ├── DiscordLoggerOptions.cs
    └── DiscordOptionsValidator.cs
```

**Configurações Principais:**
- Webhook URL(s)
- Username e Avatar customizados
- Rate limiting
- Retry policy
- Timeout
- Buffer settings

---

### 2.2 Discord Webhook Client
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `DiscordWebhookClient` - Cliente HTTP especializado
- [ ] Retry policy
- [ ] Rate limiting (Discord API limits)

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Client/
│   ├── DiscordWebhookClient.cs
│   └── RateLimiter.cs
└── Policies/
    └── RetryPolicy.cs
```

**Features:**
- Rate limit: 5 requests/2 seconds (Discord)
- Retry: 3 tentativas com backoff exponencial (2s, 4s, 8s)
- Timeout: 30 segundos

---

### 2.3 Message Formatter (Embeds)
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `DiscordMessageFormatter` - Formatação de embeds
- [ ] Mapeamento de cores por LogLevel
- [ ] Emojis personalizáveis por nível
- [ ] Formatação de campos (timestamp, category, message)
- [ ] Suporte a markdown do Discord
- [ ] Truncamento inteligente (limite 6000 chars)

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Formatting/
│   ├── DiscordMessageFormatter.cs
│   ├── EmbedBuilder.cs
│   ├── ColorMapper.cs
│   └── EmojiMapper.cs
└── Models/
    ├── DiscordEmbed.cs
    ├── EmbedField.cs
    └── EmbedColor.cs
```

**Esquema de Cores:**
| LogLevel | Cor (Hex) | Emoji |
|----------|-----------|-------|
| Trace | `#6C757D` (Cinza) | 🔍 |
| Debug | `#17A2B8` (Ciano) | 🐛 |
| Information | `#28A745` (Verde) | ℹ️ |
| Warning | `#FFC107` (Amarelo) | ⚠️ |
| Error | `#DC3545` (Vermelho) | ❌ |
| Critical | `#6F1AB6` (Roxo escuro) | 🔥 |

---

### 2.4 Suporte a Múltiplos Webhooks
**Prioridade:** Média | **Status:** 📝 Planejado

**Componentes:**
- [ ] Múltiplos webhooks por LogLevel
- [ ] Configuração declarativa
- [ ] Fallback webhook (se webhook específico falhar)

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
└── Configuration/
    ├── WebhookConfiguration.cs
    └── WebhookCollection.cs
```

**Exemplo de Configuração:**
```json
{
  "Logging": {
    "Discord": {
      "Webhooks": {
        "Critical": "https://discord.com/api/webhooks/.../critical",
        "Error": "https://discord.com/api/webhooks/.../errors",
        "Warning": "https://discord.com/api/webhooks/.../warnings",
        "Information": "https://discord.com/api/webhooks/.../info",
        "Default": "https://discord.com/api/webhooks/.../default"
      }
    }
  }
}
```

---

### 2.5 Webhook Routing (Roteamento Inteligente)
**Prioridade:** Média | **Status:** 📝 Planejado

**Componentes:**
- [ ] `WebhookRouter` - Roteamento baseado em categoria/classe
- [ ] `WebhookRoutingBuilder` - API builder pattern
- [ ] Suporte a padrões (wildcards, regex)
- [ ] Roteamento condicional (predicates)
- [ ] Prioridade de rotas

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Routing/
│   ├── WebhookRouter.cs
│   ├── WebhookRoutingBuilder.cs
│   ├── RoutingRule.cs
│   └── RouteMatch.cs
└── Extensions/
    └── RoutingExtensions.cs
```

**Exemplo de Uso (Builder Pattern):**
```csharp
logging.AddDiscord(discord =>
{
    discord.ConfigureRouting(routing =>
    {
        routing
            .RouteCategory("MyApp.Payment.*", "https://discord.../payments")
            .RouteCategory("MyApp.Auth.*", "https://discord.../auth")
            .RouteLevel(LogLevel.Critical, "https://discord.../critical")
            .RouteWhen(
                log => log.Exception is SqlException,
                "https://discord.../database"
            )
            .RouteDefault("https://discord.../default");
    });
});
```

---

### 2.6 Roteamento por Atributo
**Prioridade:** Baixa | **Status:** ✅ Concluído

**Componentes:**
- [x] `[DiscordWebhook]` attribute
- [x] Reflection-based routing
- [x] Cache de atributos
- [x] Suporte a herança de atributos

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Attributes/
│   └── DiscordWebhookAttribute.cs ✅
└── Routing/
    └── AttributeBasedRouter.cs ✅
```

**Exemplo de Uso:**
```csharp
[DiscordWebhook("https://discord.com/api/webhooks/.../payments")]
public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    
    public void ProcessPayment()
    {
        _logger.LogInformation("Processing payment");
        // Logs vão para o webhook de payments
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

---

### 2.7 Anexo de Arquivos para Mensagens Grandes
**Prioridade:** Média | **Status:** 📝 Planejado

**Componentes:**
- [ ] Detecção de mensagens grandes (> 2000 chars no description)
- [ ] Geração de arquivo `.txt` ou `.log`
- [ ] Upload via multipart/form-data
- [ ] Truncamento inteligente com link para arquivo
- [ ] Suporte especial para stack traces completas

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
├── Attachments/
│   ├── AttachmentBuilder.cs
│   ├── StackTraceAttachment.cs
│   └── LogFileGenerator.cs
└── Models/
    └── DiscordAttachment.cs
```

**Lógica:**
1. Se mensagem < 2000 chars → Embed normal
2. Se mensagem entre 2000-4000 chars → Embed + campo extra
3. Se mensagem > 4000 chars → Embed resumido + arquivo anexo

**Exemplo:**
```
📋 Stack Trace completo disponível no arquivo anexado
[Arquivo: error-2024-01-15-143022.txt (15.3 KB)]
```

---

### 2.8 Extension Methods (Configuração)
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] `AddDiscord()` extension para `ILoggingBuilder`
- [ ] Configuração fluente completa
- [ ] Overloads para diferentes cenários
- [ ] Validação em tempo de configuração

**Entregáveis:**
```
src/Elefanti.Logging.Discord/
└── Extensions/
    └── DiscordLoggingExtensions.cs
```

**Exemplos de Uso:**

**Básico:**
```csharp
logging.AddDiscord(options =>
{
    options.WebhookUrl = "https://discord.com/api/webhooks/...";
});
```

**Avançado:**
```csharp
logging.AddDiscord(discord =>
{
    discord.WebhookUrl = "https://discord.com/api/webhooks/.../default";
    discord.Username = "MyApp Logger";
    discord.AvatarUrl = "https://example.com/logo.png";
    
    discord.ConfigureRetry(retry =>
    {
        retry.MaxAttempts = 3;
        retry.BackoffMultiplier = 2.0;
    });
    
    discord.ConfigureRateLimit(limit =>
    {
        limit.MaxRequests = 5;
        limit.TimeWindow = TimeSpan.FromSeconds(2);
    });
    
    discord.ConfigureFormatting(format =>
    {
        format.IncludeTimestamp = true;
        format.IncludeCategory = true;
        format.IncludeEventId = true;
        format.CustomEmojis = new Dictionary<LogLevel, string>
        {
            [LogLevel.Critical] = "🚨",
            [LogLevel.Error] = "🔴"
        };
    });
    
    discord.ConfigureAttachments(attachments =>
    {
        attachments.MessageSizeThreshold = 2000;
        attachments.AlwaysAttachStackTraces = true;
    });
});
```

---

## 📦 Fase 3: Pacotes NuGet - 📝 Planejado

### 3.1 Elefanti.Logging
**Prioridade:** Alta | **Status:** 📝 Planejado

**Tarefas:**
- [ ] Configurar metadados do pacote
- [ ] Criar README.md do NuGet
- [ ] Gerar documentação XML
- [ ] Configurar versionamento semântico
- [ ] Configurar CI/CD para publicação

**Metadados:**
```xml
<PackageId>Elefanti.Logging</PackageId>
<Version>1.0.0</Version>
<Authors>Elefanti</Authors>
<Description>Core abstractions and utilities for Elefanti logging providers</Description>
<PackageTags>logging;microsoft-extensions-logging;elefanti</PackageTags>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
```

---

### 3.2 Elefanti.Logging.Discord
**Prioridade:** Alta | **Status:** 📝 Planejado

**Tarefas:**
- [ ] Configurar metadados do pacote
- [ ] Criar README.md do NuGet
- [ ] Gerar documentação XML
- [ ] Exemplos de uso
- [ ] Configurar CI/CD para publicação

**Dependências:**
```xml
<PackageReference Include="Elefanti.Logging" Version="1.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
<PackageReference Include="Polly" Version="8.5.0" />
```

**Metadados:**
```xml
<PackageId>Elefanti.Logging.Discord</PackageId>
<Version>1.0.0</Version>
<Authors>Elefanti</Authors>
<Description>Discord provider for Elefanti.Logging with webhook support</Description>
<PackageTags>logging;discord;webhook;microsoft-extensions-logging;elefanti</PackageTags>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
```

---

## 🧪 Fase 4: Testes - 📝 Planejado

### 4.1 Testes Unitários - Core
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] Testes de interfaces e abstrações
- [ ] Testes de utilitários
- [ ] Testes de extension methods
- [ ] Testes de configuração
- [ ] Code coverage > 80%

**Entregáveis:**
```
tests/Elefanti.Logging.Tests/
├── Abstractions/
├── Utilities/
├── Extensions/
└── Configuration/
```

---

### 4.2 Testes Unitários - Discord
**Prioridade:** Alta | **Status:** 📝 Planejado

**Componentes:**
- [ ] Testes do provider
- [ ] Testes do client (com mocks)
- [ ] Testes de formatação
- [ ] Testes de roteamento
- [ ] Testes de anexos
- [ ] Code coverage > 80%

**Entregáveis:**
```
tests/Elefanti.Logging.Discord.Tests/
├── Providers/
├── Client/
├── Formatting/
├── Routing/
└── Attachments/
```

---

### 4.3 Testes de Integração
**Prioridade:** Média | **Status:** 📝 Planejado

**Componentes:**
- [ ] Testes com webhook real (Discord test server)
- [ ] Testes de rate limiting
- [ ] Testes de retry
- [ ] Testes de fallback
- [ ] Testes de performance

**Entregáveis:**
```
tests/Elefanti.Logging.Discord.Integration.Tests/
├── WebhookTests.cs
├── RateLimitingTests.cs
└── PerformanceTests.cs
```

---

## 📚 Fase 5: Documentação - 📝 Planejado

### 5.1 Documentação Técnica
**Prioridade:** Alta | **Status:** 📝 Planejado

**Tarefas:**
- [ ] README.md principal
- [ ] Guia de início rápido
- [ ] Referência de API
- [ ] Guia de configuração
- [ ] Exemplos de código
- [ ] FAQ

---

### 5.2 Samples e Exemplos
**Prioridade:** Média | **Status:** 📝 Planejado

**Tarefas:**
- [ ] Aplicação console de exemplo
- [ ] Aplicação ASP.NET Core de exemplo
- [ ] Exemplo com múltiplos webhooks
- [ ] Exemplo com roteamento
- [ ] Exemplo com anexos

**Entregáveis:**
```
samples/
├── ConsoleApp/
├── WebApi/
├── MultipleWebhooks/
├── AdvancedRouting/
└── FileAttachments/
```

---

## 🔮 Fase 6: Futuro (Outros Providers) - ⏸️ Pausado

### Providers Futuros (Não Planejados Ainda)
- ⏸️ **Elefanti.Logging.Slack**
- ⏸️ **Elefanti.Logging.Telegram**
- ⏸️ **Elefanti.Logging.Teams**
- ⏸️ **Elefanti.Logging.Email**

> **Nota:** Estes providers seguirão as mesmas abstrações do Core, facilitando a implementação futura.

---

## ✅ Checklist de Entrega

### MVP (Minimum Viable Product)
- [ ] Core com abstrações principais
- [ ] Discord provider básico
- [ ] Webhook client com retry
- [ ] Message formatter com embeds
- [ ] Configuração via extension methods
- [ ] Testes unitários básicos
- [ ] README e documentação inicial
- [ ] Publicação no NuGet

### v1.0 (Full Release)
- [ ] Todas as features da Fase 1 e 2
- [ ] Múltiplos webhooks
- [ ] Routing completo (builder + attributes)
- [ ] Anexo de arquivos
- [ ] Code coverage > 80%
- [ ] Testes de integração
- [ ] Documentação completa
- [ ] Samples funcionais

---

## 🎯 Prioridades de Implementação

### 🔴 Prioridade Crítica (Implementar Primeiro)
1. Core abstractions (Fase 1.1)
2. Integração com Microsoft.Extensions.Logging (Fase 1.2)
3. Discord provider base (Fase 2.1)
4. Discord webhook client (Fase 2.2)
5. Message formatter (Fase 2.3)
6. Extension methods básicos (Fase 1.3 + 2.8)

### 🟡 Prioridade Alta (Implementar em Seguida)
1. Múltiplos webhooks (Fase 2.4)
2. Webhook routing builder (Fase 2.5)
3. Anexo de arquivos (Fase 2.7)
4. Testes unitários (Fase 4.1 + 4.2)

### 🟢 Prioridade Média (Pode ser Posterior)
1. Roteamento por atributo (Fase 2.6)
2. Utilitários avançados (Fase 1.4)
3. Testes de integração (Fase 4.3)
4. Samples (Fase 5.2)

---

## 📝 Notas Técnicas

### Dependências Principais
- **Microsoft.Extensions.Logging** (9.0.0+)
- **Microsoft.Extensions.Logging.Abstractions** (9.0.0+)
- **Microsoft.Extensions.Http** (9.0.0+)
- **Polly** (8.5.0+) - Para retry e circuit breaker
- **System.Text.Json** (9.0.0+) - Para serialização

### Boas Práticas
- ✅ Usar async/await em todas as operações de I/O
- ✅ Implementar IDisposable corretamente
- ✅ Evitar bloqueio de threads
- ✅ Arquivos em UTF-8
- ✅ Usar CancellationToken
- ✅ Logging estruturado
- ✅ Configuração imutável
- ✅ Thread-safe
- ✅ Testes automatizados

### Performance Considerations
- Buffer de logs (evitar envio síncrono)
- Background queue para processamento
- Rate limiting local (evitar hits desnecessários)
- Connection pooling do HttpClient
- Cache de rotas de routing

---

## 🤝 Contribuições

Este roadmap está aberto a sugestões e melhorias. Ao implementar, marque os itens com ✅ e adicione notas se necessário.

---

**Última Atualização:** 2025-10-21 | **Versão do Roadmap:** 1.0
