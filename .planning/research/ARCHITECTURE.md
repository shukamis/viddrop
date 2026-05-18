# Architecture Research: VidDrop

**Researched:** 2026-05-18 | **Confidence:** HIGH (.NET patterns) / MEDIUM (yt-dlp flags)

## Component Boundaries

7 logical components — single .csproj, no DI container, no MVVM framework.

MainForm (UI only)
  -> DownloadCoordinator (orchestration, owns CancellationTokenSource)
       -> MetadataFetcher  (yt-dlp --dump-single-json)
       -> DownloadEngine   (yt-dlp download + progress)
       -> ThumbnailService (HttpClient image fetch)
       -> YtDlpProcessRunner (sole owner of System.Diagnostics.Process)
       -> ToolLocator (resolve yt-dlp.exe and ffmpeg.exe paths)

- MainForm: render UI, no process spawning or output parsing
- DownloadCoordinator: sequence job, own CTS, clean API to form
- MetadataFetcher: yt-dlp --dump-single-json, deserialize VideoMetadata, enforce timeout
- DownloadEngine: build yt-dlp args, parse progress into DownloadProgress
- ThumbnailService: shared HttpClient, decode Image, placeholder fallback
- YtDlpProcessRunner: ONLY component touching System.Diagnostics.Process
- ToolLocator: resolve paths from AppContext.BaseDirectory, verify at startup

## Data Flow

1. URL pasted -> OnUrlChanged (debounced ~500ms)
2. METADATA FETCH: MainForm -> DownloadCoordinator.FetchMetadataAsync
   -> yt-dlp --dump-single-json --no-playlist <url>
   -> VideoMetadata { Title, ThumbnailUrl, Duration, Formats[] }
3. THUMBNAIL FETCH (concurrent): ThumbnailService -> HttpClient -> PictureBox
4. USER CHOICE: MP3|MP4 + quality -> DownloadCoordinator.DownloadAsync
5. DOWNLOAD: DownloadEngine builds args -> YtDlpProcessRunner
   -> yt-dlp with --newline progress -> ffmpeg merge internally
   -> stdout -> ProgressParser -> IProgress<DownloadProgress> -> UI thread
6. FILE OUT: exit 0 -> file in %USERPROFILE%\Downloads\VidDrop\
   Non-zero exit -> ErrorClassifier -> DownloadException -> friendly message

IMPORTANT: Metadata fetch and download are TWO SEPARATE yt-dlp invocations.

## Threading Model

Recommendation: Task + async/await with IProgress<T> — NOT BackgroundWorker. (HIGH)

Work is I/O-bound and process-bound. Progress<T> auto-marshals to the UI SynchronizationContext.

Pattern:
- btnDownload_Click is async void (sanctioned for WinForms event handlers)
- new Progress<DownloadProgress>(OnProgress) captures UI SynchronizationContext at construction
- OnProgress runs on UI thread automatically — no Control.Invoke needed
- .ConfigureAwait(false) in Core/Process code to avoid unnecessary UI-thread hops
- One job at a time — single CTS, disable Download while running

## Process Management

Highest-risk area. YtDlpProcessRunner is sole owner of System.Diagnostics.Process. (HIGH)

ProcessStartInfo: UseShellExecute=false, RedirectStdout/Stderr=true, CreateNoWindow=true
StandardOutputEncoding=UTF8, StandardErrorEncoding=UTF8 (prevents non-ASCII title corruption)
Use ArgumentList (not Arguments string) — avoids URL & injection bugs
Pass --ffmpeg-location <binDir> so yt-dlp finds bundled ffmpeg [VERIFY]

Avoid deadlock: drain BOTH stdout and stderr concurrently via BeginOutputReadLine + BeginErrorReadLine.
NEVER read one stream to completion before starting the other.

Cancellation: Kill(entireProcessTree: true) — kills yt-dlp AND grandchild ffmpeg
FormClosing: cancel active CTS + kill-tree before form closes
After Kill: await WaitForExitAsync to release file handles
Delete *.part / *.ytdl partial files on cancel/error (best-effort)

Timeouts: metadata fetch ~20-30s via CancelAfter; download: optional stall timeout ~60s

## Error Handling Architecture

Chain: yt-dlp non-zero exit -> ProcessResult -> ErrorClassifier -> DownloadException -> MainForm catch -> inline error panel

ErrorClassifier patterns [VERIFY against current yt-dlp stderr]:
- Unsupported URL -> This link is not supported
- Video unavailable / private -> This video is private or unavailable
- not available in your country -> Blocked in your region
- Sign in to confirm your age -> Age-restricted
- HTTP Error 429 -> Too many requests. Wait a few minutes
- ffmpeg not found -> Required component missing. Try reinstalling
- No space left -> Not enough disk space
- (anything else) -> Something went wrong. [Show details]

UI: inline error region (not modal). UserMessage prominent. Raw stderr behind Show details expander.
Startup check: ToolLocator verifies yt-dlp.exe + ffmpeg.exe exist; modal error + exit if missing.

## Suggested Build Order

Phase 1: YtDlpProcessRunner + ToolLocator (no UI)
  Test via console harness. Highest risk — pipe deadlock, zombie ffmpeg, encoding.

Phase 2: MetadataFetcher + VideoMetadata model
  Console harness: YouTube + Twitter + bad URL. Depends on Phase 1.

Phase 3: DownloadEngine + ProgressParser + DownloadOptions
  Download short MP4 + MP3, verify progress parsing + ffmpeg merge. Depends on Phase 1.

Phase 4: DownloadCoordinator + MainForm (minimal)
  URL box, Fetch, format/quality combo, Download, progress bar. Depends on Phase 2+3.

Phase 5: ThumbnailService + preview polish
  PictureBox, loading spinner, GDI disposal. Depends on Phase 4.

Phase 6: Error handling + robustness
  ErrorClassifier, inline error panel, cancel UX, partial-file cleanup. Depends on Phase 5.

Phase 7: XP aesthetic + Inno Setup packaging
  Control styling, icon, bundling yt-dlp + ffmpeg. Depends on Phase 6.

Dependency graph: P1->P2 and P1->P3; (P2+P3)->P4->P5->P6->P7

## Project File Layout

VidDrop/
  VidDrop.sln
  VidDrop.csproj  (net8.0-windows, SelfContained=true)
  Program.cs      (entry; ToolLocator startup check)
  UI/             (VidDrop.UI — only WinForms code)
    MainForm.cs, MainForm.Designer.cs, Controls/XpProgressBar.cs
  Core/           (VidDrop.Core — UI-free, testable)
    DownloadCoordinator.cs, MetadataFetcher.cs, DownloadEngine.cs
    ThumbnailService.cs, ProgressParser.cs
  Process/        (VidDrop.Process)
    YtDlpProcessRunner.cs  <- ONLY file with System.Diagnostics.Process
    ProcessResult.cs, ToolLocator.cs
  Models/         (VidDrop.Models — plain POCOs)
    VideoMetadata.cs, DownloadOptions.cs, DownloadProgress.cs
    MediaFormat.cs (enum Mp3/Mp4), QualityLevel.cs (enum Best/P1080/P720/P480/P360)
  Errors/         (VidDrop.Errors)
    DownloadException.cs, ErrorCategory.cs, ErrorClassifier.cs
  Resources/
    placeholder-thumb.png, app.ico
  Tools/          (Copy to Output Directory)
    yt-dlp.exe, ffmpeg.exe

---
*Research complete: 2026-05-18*

