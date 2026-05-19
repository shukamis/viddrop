<div align="center">

# VidDrop

**Baixador de vídeos para Windows limpo, rápido e com alma Frutiger Aero.**

*A clean, fast video downloader for Windows with a Frutiger Aero soul.*

[![Release](https://img.shields.io/github/v/release/shukamis/viddrop?style=flat-square&color=5BA4E6&label=versão)](https://github.com/shukamis/viddrop/releases/latest)
[![Platform](https://img.shields.io/badge/plataforma-Windows%2010%2F11-blue?style=flat-square&color=5BA4E6)](https://github.com/shukamis/viddrop/releases/latest)
[![License](https://img.shields.io/badge/licença-MIT-blue?style=flat-square&color=5BA4E6)](LICENSE)

Cole uma URL. Escolha o formato. Pronto.

---

</div>

## O que faz

O VidDrop baixa vídeos da internet sem complicação. Sem extensão de navegador, sem Python, sem terminal. Cole a URL e clique em baixar.

Suporta **YouTube · Twitter/X · Instagram · Facebook · TikTok** — e qualquer outra plataforma que o yt-dlp suporte.

<div align="center">

| Recurso | Detalhes |
|---|---|
| Formatos | MP4 (vídeo) · MP3 (áudio) |
| Qualidade | Melhor, 1080p, 720p, 480p, 360p |
| Bitrate de áudio | 320 · 192 · 128 · 64 kbps |
| Modo em lote | Carrega um `.txt` com várias URLs |
| Pré-visualização | Thumbnail + título automáticos ao colar |
| Progresso | Barra multi-etapa com cancelamento |
| Atualizações | Auto-update silencioso via Velopack |
| Pasta de saída | Salva direto em `Downloads\` |

</div>

---

## Como usar

### Modo fácil — só instalar e usar

1. Baixe o instalador na página de [**Releases**](https://github.com/shukamis/viddrop/releases/latest)
2. Execute o instalador — não precisa de nada além do próprio app (tudo já vem dentro)
3. Abra o VidDrop
4. Cole uma URL de vídeo no campo de texto
5. Escolha o formato (MP4 ou MP3) e a qualidade
6. Clique em **Baixar** — o arquivo vai direto para a pasta `Downloads`

> Compatível com Windows 10 e 11 (64-bit). Sem dependências externas.

### Baixar vários vídeos de uma vez

1. Crie um arquivo `.txt` com uma URL por linha
2. Clique no botão **lote** dentro do app
3. Selecione o arquivo — o download começa automaticamente

---

## Compilar do zero

> *Build from source*

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Windows (WinForms exige Windows)
- [yt-dlp.exe](https://github.com/yt-dlp/yt-dlp/releases/latest) + [ffmpeg.exe](https://github.com/BtbN/FFmpeg-Builds/releases) — coloque ambos em `Tools\`

### Rodar

```
dotnet run
```

### Publicar (auto-contido)

```
dotnet publish -c Release -r win-x64 --self-contained
```

Os binários em `Tools\` são copiados automaticamente para a saída (configurado no `.csproj`).

---

## Estrutura do projeto

```
VidDrop/
├── UI/          — MainForm, AeroButton, AeroProgressBar
├── Core/        — DownloadCoordinator, MetadataFetcher, DownloadEngine,
│                  ProgressParser, SelfUpdater, UpdateChecker
├── Process/     — YtDlpProcessRunner (único dono de Process), ToolLocator
├── Models/      — VideoMetadata, DownloadOptions, DownloadProgress, MediaFormat
├── Errors/      — DownloadException, ErrorClassifier
├── Installer/   — script de build Velopack
└── Tools/       — yt-dlp.exe + ffmpeg.exe (não incluídos no repo — adicionar manualmente)
```

---

## Tecnologias

- **C# / .NET 8** — WinForms, publicação auto-contida win-x64
- **[YoutubeDLSharp](https://github.com/Bluegrams/YoutubeDLSharp)** — wrapper para yt-dlp
- **[yt-dlp](https://github.com/yt-dlp/yt-dlp)** — motor de download (embutido)
- **[ffmpeg](https://ffmpeg.org/)** — mux/transcode (embutido, build LGPL)
- **[Velopack](https://velopack.io/)** — auto-update + instalador

---

## Licença

MIT — veja [LICENSE](LICENSE).

yt-dlp e ffmpeg são distribuídos sob suas respectivas licenças (Unlicense / LGPL-2.1).
