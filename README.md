<div align="center">

# VidDrop

**A clean, fast video downloader for Windows with a Frutiger Aero soul.**

[![Release](https://img.shields.io/github/v/release/shukamis/viddrop?style=flat-square&color=5BA4E6&label=version)](https://github.com/shukamis/viddrop/releases/latest)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue?style=flat-square&color=5BA4E6)](https://github.com/shukamis/viddrop/releases/latest)
[![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square&color=5BA4E6)](LICENSE)

Paste a URL. Pick a format. Done.

---

</div>

## What it does

VidDrop downloads videos from the web with zero friction. No browser extension, no Python, no CLI. Just paste a URL and hit download.

Supports **YouTube · Twitter/X · Instagram · Facebook · TikTok** — and anything else yt-dlp handles.

<div align="center">

| Feature | Details |
|---|---|
| Formats | MP4 (video) · MP3 (audio) |
| Quality | Best, 1080p, 720p, 480p, 360p |
| Audio bitrate | 320 · 192 · 128 · 64 kbps |
| Batch mode | Load a `.txt` file of URLs |
| Auto preview | Thumbnail + title on paste |
| Progress | Multi-stage bar with cancel |
| Updates | Silent auto-update via Velopack |
| Output | Saved to `Downloads\` folder |

</div>

---

## Download

Grab the latest installer from [**Releases**](https://github.com/shukamis/viddrop/releases/latest).

> Requires Windows 10 or 11 (x64). No extra installs — everything is bundled.

Run the installer → launch VidDrop → paste a URL. That's it.

---

## How it works

1. **Paste** a URL from YouTube, Twitter/X, Instagram, Facebook, or TikTok
2. The app **fetches** title and thumbnail automatically (800 ms debounce)
3. **Pick** format (MP4 / MP3) and quality
4. Hit **Baixar** — file lands in your `Downloads` folder

For multiple videos, hit the **batch** button and load a `.txt` file with one URL per line.

---

## Build from source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Windows (WinForms requires Windows)
- [yt-dlp.exe](https://github.com/yt-dlp/yt-dlp/releases/latest) + [ffmpeg.exe](https://github.com/BtbN/FFmpeg-Builds/releases) — place both in `Tools\`

### Run

```
dotnet run
```

### Publish (self-contained)

```
dotnet publish -c Release -r win-x64 --self-contained
```

The `Tools\` binaries are bundled automatically (Copy to Output Directory is set in the project file).

---

## Project structure

```
VidDrop/
├── UI/          — MainForm, AeroButton, AeroProgressBar
├── Core/        — DownloadCoordinator, MetadataFetcher, DownloadEngine,
│                  ProgressParser, SelfUpdater, UpdateChecker
├── Process/     — YtDlpProcessRunner (sole Process owner), ToolLocator
├── Models/      — VideoMetadata, DownloadOptions, DownloadProgress, MediaFormat
├── Errors/      — DownloadException, ErrorClassifier
├── Installer/   — Velopack build script
└── Tools/       — yt-dlp.exe + ffmpeg.exe (not in repo — add manually)
```

---

## Stack

- **C# / .NET 8** — WinForms, self-contained win-x64 publish
- **[YoutubeDLSharp](https://github.com/Bluegrams/YoutubeDLSharp)** — yt-dlp wrapper
- **[yt-dlp](https://github.com/yt-dlp/yt-dlp)** — download engine (bundled)
- **[ffmpeg](https://ffmpeg.org/)** — mux/transcode (bundled, LGPL build)
- **[Velopack](https://velopack.io/)** — auto-update + installer

---

## License

MIT — see [LICENSE](LICENSE).

yt-dlp and ffmpeg are distributed under their respective licenses (Unlicense / LGPL-2.1).
