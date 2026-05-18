# Stack Research: VidDrop

**Researched:** 2026-05-18 | **Overall confidence:** MEDIUM (web verification unavailable)

## Recommended Stack

### .NET / WinForms

Use **.NET 8 LTS** (verify if .NET 10 LTS is now current), WinForms targeting `net8.0-windows`, published **self-contained + single-file** for `win-x64`.

**Rationale:** A public consumer app must not fail with ".NET runtime not installed." Self-contained single-file adds ~60-90 MB but eliminates the most common startup support issue. .NET Framework 4.8 yields a tiny installer but is maintenance-mode. Keep `PublishTrimmed=false` (WinForms trims unreliably). Use `AppContext.BaseDirectory` to locate bundled binaries (single-file makes `Assembly.Location` empty).

### yt-dlp Integration

Use the **YoutubeDLSharp** NuGet package (Bluegrams) rather than hand-rolled `Process.Start`. It wraps a `yt-dlp.exe` you supply, exposes `RunVideoDownload`/`RunAudioDownload`/`RunVideoDataFetch`, typed `OptionSet`, `IProgress<DownloadProgress>`, and `YoutubeDLPath`/`FFmpegPath` properties.

Ship the official standalone `yt-dlp.exe` (PyInstaller build, no Python needed). Plan for updates — yt-dlp releases monthly and old builds break on YouTube.

### ffmpeg Bundling

Bundle a single static **ffmpeg.exe** (essentials build from gyan.dev or BtbN) in a `bin\` subfolder. Point yt-dlp via `--ffmpeg-location` / `FFmpegPath`. Only `ffmpeg.exe` is required; `ffprobe.exe` optional; skip `ffplay.exe`.

**Legal flag:** GPL-flagged ffmpeg builds impose redistribution obligations — requires legal review. LGPL-variant preferred.

### Preview & Metadata

`yt-dlp --dump-single-json --skip-download <url>` via `RunVideoDataFetch`, on a background thread, with `async/await` resuming on the UI `SynchronizationContext` (no manual `Invoke` needed). Use one static `HttpClient` for thumbnail download into a `PictureBox`. Support `CancellationToken` to cancel stale fetches when the user pastes a new URL.

### Progress Parsing

Use YoutubeDLSharp's `IProgress<DownloadProgress>` (exposes `State`, `Progress` 0.0-1.0, speed, ETA). Handle the two-phase download (video, audio, then ffmpeg merge) with a state machine; use an indeterminate bar during post-processing.

### Installer (Inno Setup)

Inno Setup **6.x**, **per-user install** (`PrivilegesRequired=lowest`, `DefaultDirName={localappdata}\Programs\VidDrop`) so there's no UAC prompt and yt-dlp can self-update. Bundle the full publish output plus `bin\yt-dlp.exe` and `bin\ffmpeg.exe`. Use `Compression=lzma2/ultra64`, `SolidCompression=yes`. Create `%USERPROFILE%\Downloads\VidDrop` at app first-run, not in the installer.

### WinForms XP Aesthetic

Remove `Application.EnableVisualStyles()` from `Program.cs` — standard controls render in classic gray 3D style (pre-visual-styles theme). Reinforce with Tahoma 8pt font, `FixedDialog` borders, hidden size grip.

**Note:** True XP Luna (blue gradients) requires owner-drawn controls (large effort). Classic-gray is cheap and authentic to the era. Confirm with stakeholder whether "XP aesthetic" means classic-gray or Luna-blue.

## What NOT to Use

- .NET Framework 4.8 as primary target — maintenance-mode, limited modern async support
- Framework-dependent .NET 8 deployment — breaks on machines without runtime
- `PublishTrimmed=true` — WinForms trims unreliably, causes runtime errors
- Hand-rolled stdout-regex progress parsing — fragile, breaks on yt-dlp format changes
- The original `youtube-dl` — abandoned, use yt-dlp fork instead
- Pure-C# extractors (VideoLibrary, YoutubeExplode) — only work for YouTube, not multi-platform
- Bundling Python — massively inflates installer size
- Full ffmpeg build with ffplay.exe — unnecessary bloat
- Per-machine / Program Files install — requires UAC elevation, causes self-update permission issues
- Third-party WinForms skinning libraries — fragile, often abandoned

## Open Verification Items

1. Current LTS .NET version + real self-contained publish size
2. YoutubeDLSharp NuGet version, health, and API method names
3. Latest yt-dlp release + recommended update strategy
4. ffmpeg build source + confirm LGPL vs GPL license
5. Latest Inno Setup 6.x version
6. Stakeholder definition of "XP aesthetic" — classic-gray vs Luna-blue

## Confidence Levels

| Recommendation | Confidence |
|---|---|
| .NET 8 self-contained WinForms | HIGH |
| YoutubeDLSharp for yt-dlp integration | HIGH |
| ffmpeg bundled in bin\ subfolder | HIGH |
| Per-user Inno Setup install | HIGH |
| Remove EnableVisualStyles for XP look | HIGH |
| Exact version numbers | LOW — must verify |
