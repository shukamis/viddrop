# VidDrop — Project Guide

## Project

Native Windows desktop video downloader app. C# .NET 8 + WinForms, Frutiger Aero aesthetic, yt-dlp + ffmpeg bundled, Inno Setup installer. Phase 7 adds AI-powered viral clip generation.

See .planning/PROJECT.md for full context.

## GSD Workflow

This project uses the Get Shit Done (GSD) workflow.

### Phase execution order

/gsd-plan-phase N — plan a phase before executing
/gsd-execute-phase N — execute a planned phase
/gsd-verify-work N — verify phase deliverables

### Current state

See .planning/STATE.md for current phase status.
See .planning/ROADMAP.md for all phases and requirements.

## Stack

- Language: C# (.NET 8, net8.0-windows)
- UI: WinForms (Frutiger Aero custom owner-drawn controls)
- Download engine: YoutubeDLSharp NuGet + bundled yt-dlp.exe
- Audio/video: bundled ffmpeg.exe (LGPL build)
- Installer: Inno Setup 6.x (per-user, no UAC)
- AI providers (Phase 7): Whisper local, OpenAI API, Gemini API, Anthropic Claude API

## Key Constraints

- Windows only (WinForms is not truly cross-platform)
- Self-contained publish (SelfContained=true, win-x64) — no runtime dependency
- Use AppContext.BaseDirectory (not Assembly.Location) to locate bundled binaries
- YtDlpProcessRunner is the ONLY class allowed to use System.Diagnostics.Process
- Always BeginOutputReadLine + BeginErrorReadLine (never sync reads — deadlock risk)
- Always Kill(entireProcessTree: true) on cancel (kills grandchild ffmpeg too)
- Always StandardOutputEncoding = UTF8 (prevents non-ASCII title corruption)
- Use ProcessStartInfo.ArgumentList (not Arguments string) to avoid URL injection

## Architecture

Single .csproj, 7 logical components:

  UI/ — MainForm only. No process spawning, no output parsing.
  Core/ — DownloadCoordinator, MetadataFetcher, DownloadEngine, ThumbnailService, ProgressParser
  Process/ — YtDlpProcessRunner (sole Process owner), ProcessResult, ToolLocator
  Models/ — VideoMetadata, DownloadOptions, DownloadProgress, MediaFormat, QualityLevel
  Errors/ — DownloadException, ErrorCategory, ErrorClassifier
  Resources/ — placeholder-thumb.png, app.ico
  Tools/ — yt-dlp.exe, ffmpeg.exe (Copy to Output Directory)

## Commit style

feat: short description
bug: short description
docs: short description
refactor: short description

