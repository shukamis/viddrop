# VidDrop — Video & Audio Downloader

## What This Is

App desktop nativo para Windows que baixa vídeos e áudios de qualquer URL — YouTube, Twitter/X, Instagram, Facebook e URLs genéricas. O usuário cola a URL, vê um preview com thumbnail e título, escolhe o formato (MP3 ou MP4) e a qualidade, e baixa. Interface com estética clássica Windows XP: controles nativos com relevo, bordas reais, visual de programa dos anos 2000.

## Core Value

Colar uma URL e ter o arquivo baixado em segundos, sem instalar mais nada além do próprio app.

## Requirements

### Validated

(None yet — ship to validate)

### Active

- [ ] Usuário pode colar URL e ver preview (thumbnail + título) antes de baixar
- [ ] Suporte a YouTube, Twitter/X, Instagram, Facebook e URLs genéricas de vídeo
- [ ] Opção de baixar como MP4 (vídeo) com seleção de qualidade (1080p, 720p, 480p, 360p)
- [ ] Opção de baixar como MP3 (áudio extraído)
- [ ] yt-dlp e ffmpeg embutidos no instalador — sem dependências externas
- [ ] Arquivos salvos em pasta padrão fixa (ex: %USERPROFILE%\Downloads\VidDrop)
- [ ] Barra de progresso durante o download
- [ ] Instalador Windows (.exe) gerado com Inno Setup

### Out of Scope

- Histórico de downloads persistente — cada sessão começa do zero
- Fila de múltiplos downloads simultâneos — v2
- Suporte a playlists inteiras — v2
- Login em contas (Instagram privado, etc.) — v2
- App para Mac/Linux — Windows first

## Context

- Motor de download: yt-dlp (bundled como executável estático)
- Conversão de áudio: ffmpeg (bundled junto com yt-dlp)
- Linguagem: C# (.NET 8) com WinForms para UI nativa
- Estética: Windows XP clássico — visual pré-Vista com controles com relevo
- Instalador: Inno Setup — gera VidDrop_Setup.exe com tudo embutido
- Público: produto público, qualquer usuário Windows pode baixar e instalar

## Constraints

- Plataforma: Windows only
- Distribuição: instalador auto-suficiente — yt-dlp e ffmpeg embutidos
- UI: deve parecer um programa dos anos 2000 — não usar design system moderno
- Tamanho: instalador ~60-80MB por causa do ffmpeg bundled — aceitável

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| C# + WinForms | Native Windows, XP aesthetic natural, gera .exe real | — Pending |
| yt-dlp bundled | Usuário instala só o app — zero fricção, sem dependências | — Pending |
| Sem histórico | Simplicidade — app de uso pontual | — Pending |
| Preview antes de baixar | Confirmar URL certa antes de gastar tempo baixando | — Pending |
| Pasta fixa padrão | Menos configuração, usuário sabe onde achar | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition:**
1. Requirements invalidated? Move to Out of Scope with reason
2. Requirements validated? Move to Validated with phase reference
3. New requirements emerged? Add to Active
4. Decisions to log? Add to Key Decisions

**After each milestone:**
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?

---
*Last updated: 2026-05-18 after initialization*
