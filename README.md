# 🐘 Elefanti.Logging

**Elefanti.Logging** é um pacote de integração com o `Microsoft.Extensions.Logging` desenvolvido pela **Elefanti**, com foco em extensibilidade e comunicação entre sistemas de monitoramento e chat corporativo.

Este projeto permite o envio de logs da sua aplicação .NET para múltiplos destinos, como **Discord**, **Slack**, **Telegram** (e outros no futuro), através de provedores especializados, mantendo compatibilidade total com o ecossistema do `ILogger` da Microsoft.

---

## 📦 Estrutura do Projeto

| Projeto | Descrição |
|----------|------------|
| **Elefanti.Logging** | Núcleo principal contendo interfaces, abstrações e utilitários compartilhados entre todos os provedores. |
| **Elefanti.Logging.Discord** | Provider para envio de logs ao **Discord**, utilizando Webhooks e integração nativa com `ILoggerProvider`. |
| **Elefanti.Logging.Slack** | Não planejado. |
| **Elefanti.Logging.Telegram** | Não planejado. |

---

## 🚀 Instalação

### Pacote principal (Core)
```bash
dotnet add package Elefanti.Logging
