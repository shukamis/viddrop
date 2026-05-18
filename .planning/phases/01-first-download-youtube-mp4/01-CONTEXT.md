# Phase 1 Context: First Download (YouTube MP4)

**Phase:** 1 — First Download (YouTube MP4)
**Date:** 2026-05-18
**Goal:** User can paste a YouTube URL into VidDrop and download the video as an MP4 file to their Downloads folder.

## Domain

Building the functional core: C# project structure, yt-dlp process engine, and minimal UI that downloads a YouTube MP4 end-to-end.

## Decisions

### yt-dlp Integration

- **Wrapper:** Custom YtDlpProcessRunner class (NOT YoutubeDLSharp NuGet). Full control, zero third-party dependency.
- **Location:** Single .csproj, folder Process/ (namespace VidDrop.Process). No separate project/solution.
- **Progress parsing:** Pass --progress-template to yt-dlp for deterministic output. Template: "%(progress._percent_str)s|%(progress._speed_str)s|%(progress._eta_str)s". Parse by splitting on |. Robust across yt-dlp updates.
- **Streams:** BeginOutputReadLine + BeginErrorReadLine (both async, concurrent). Never ReadToEnd.
- **Cancellation:** Kill(entireProcessTree: true) on cancel. Prevents orphaned ffmpeg.
- **Encoding:** StandardOutputEncoding = StandardErrorEncoding = Encoding.UTF8.
- **Args:** ProcessStartInfo.ArgumentList (not Arguments string). Prevents URL & injection.
- **ffmpeg path:** Always pass --ffmpeg-location resolving from AppContext.BaseDirectory.

### Project Structure

Single solution, single .csproj (net8.0-windows, WinForms, SelfContained=true, win-x64):

  VidDrop/
    UI/        — MainForm only (no process, no parsing)
    Core/      — DownloadCoordinator, DownloadEngine, ProgressParser
    Process/   — YtDlpProcessRunner, ProcessResult, ToolLocator
    Models/    — DownloadOptions, DownloadProgress, QualityLevel, MediaFormat
    Errors/    — DownloadException, ErrorCategory, ErrorClassifier
    Resources/ — placeholder-thumb.png, app.ico
    Tools/     — yt-dlp.exe, ffmpeg.exe (Copy to Output Directory)

### yt-dlp Binary Source

- **Dev and production:** Bundled in Tools/ folder. Copied to output directory automatically.
- **AV false positives:** Deferred to Phase 6 (code signing, EV cert, AV pre-submission).
- **Rationale:** Bundled = zero user friction. Staleness handled by ENG-01 self-update in Phase 6.

### Phase 1 UI

- **Window:** FixedDialog, ~520x280px. No Frutiger Aero styling yet (Phase 6).
- **Controls:** URL TextBox, Quality ComboBox, Download Button, ProgressBar, StatusLabel.
- **Quality ComboBox:** Best / 1080p / 720p / 480p / 360p. Populated at startup (not after probe — probe is Phase 3).
- **Default quality:** Best.
- **State machine:** Idle → Loading → Downloading → Done/Error.
- **No preview yet:** Phase 3 adds thumbnail/title. Phase 1 is URL + quality + download.

### Quality Selection (DL-01 in Phase 1)

Quality maps to yt-dlp -f selectors:
  Best:  bestvideo+bestaudio/best
  1080p: bestvideo[height<=1080]+bestaudio/best[height<=1080]
  720p:  bestvideo[height<=720]+bestaudio/best[height<=720]
  480p:  bestvideo[height<=480]+bestaudio/best[height<=480]
  360p:  bestvideo[height<=360]+bestaudio/best[height<=360]

Always add /best fallback for platforms without separate streams.
Pass --merge-output-format mp4 to ensure MP4 container.

### Output Folder (ENG-04)

- Path: %USERPROFILE%\Downloads\VidDrop
- Created at first download attempt via Directory.CreateDirectory (idempotent).
- Filename: yt-dlp default %(title)s.%(ext)s with --restrict-filenames for Windows safety.

### Startup Verification (ENG-03)

- ToolLocator checks yt-dlp.exe and ffmpeg.exe at startup (AppContext.BaseDirectory + Tools\).
- If either missing: modal MessageBox "Installation appears damaged. Please reinstall VidDrop." + Application.Exit().
- Also creates output folder on first run.

### Threading

- async/await + IProgress<DownloadProgress>. No BackgroundWorker.
- new Progress<DownloadProgress>(OnProgress) captures UI SynchronizationContext.
- Download button disabled while downloading. Single CTS per job.
- FormClosing: cancel CTS + Kill(entireProcessTree) before close.

## Canonical Refs

- .planning/PROJECT.md — project context and constraints
- .planning/REQUIREMENTS.md — DL-01, PLAT-01, ENG-03, ENG-04
- .planning/research/STACK.md — .NET/WinForms/yt-dlp stack decisions
- .planning/research/ARCHITECTURE.md — component boundaries, threading, process management
- .planning/research/PITFALLS.md — critical pitfalls C1-C7 (all apply to Phase 1)

## Code Context

Greenfield — no existing code. Phase 1 creates the entire project from scratch.
Build order within phase: ToolLocator → YtDlpProcessRunner → DownloadEngine → ProgressParser → MainForm.

## Deferred Ideas

- YoutubeDLSharp NuGet as alternative wrapper — rejected for Phase 1, could revisit if custom runner proves fragile
- Separate VidDrop.Engine project — deferred, single-project sufficient at this scale
