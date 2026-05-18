# Research Summary: VidDrop

**Synthesized:** 2026-05-18 | **Overall confidence:** MEDIUM (web verification was unavailable; .NET architecture patterns are HIGH confidence, exact versions and yt-dlp flags need verification)

VidDrop is a native Windows WinForms desktop app with a classic Windows XP aesthetic that downloads videos from YouTube, Twitter/X, Instagram, and Facebook. The recommended approach is well-understood: a thin WinForms UI over a yt-dlp.exe + ffmpeg.exe engine pair bundled into a per-user Inno Setup installer. The hard parts are not the UI -- they are robust child-process management (pipe handling, process-tree kills, encoding) and the operational reality that yt-dlp goes stale within 1-3 months unless it can self-update.

The dominant risk theme across all four research files is the gap between "works on my machine" and "works for a public consumer". Three issues recur: (1) antivirus false positives on the PyInstaller-packed yt-dlp.exe, mitigated by code signing; (2) a frozen yt-dlp that self-destructs post-launch, mitigated by self-update on a writable per-user install; and (3) child-process correctness bugs (pipe deadlock, zombie ffmpeg, UTF-8 corruption) that must be solved once, early, in an isolated engine layer. Build the engine first behind a console harness, layer the UI on top, and treat error-message mapping and the multi-stage progress UX as first-class features rather than polish.

## Recommended Stack

- **.NET 8 LTS, WinForms (net8.0-windows), published self-contained single-file for win-x64** -- eliminates the most common support failure (".NET runtime not installed"); accept the ~60-90 MB size. Keep PublishTrimmed=false (WinForms trims unreliably). Verify whether a newer LTS is now current.
- **YoutubeDLSharp NuGet package** wrapping a supplied yt-dlp.exe (PyInstaller standalone, no Python) plus a single static **LGPL** ffmpeg.exe in a bin/ subfolder -- avoids fragile hand-rolled Process.Start and regex progress parsing. GPL ffmpeg builds carry redistribution obligations; prefer LGPL.
- **Inno Setup 6.x, per-user install** (PrivilegesRequired=lowest, DefaultDirName={localappdata}/Programs/VidDrop, lzma2/ultra64 compression) -- no UAC prompt and a writable folder so yt-dlp can self-update. Expected installer size ~80-100 MB.

## Table Stakes Features

Must be in v1 or users leave:

- Paste URL with automatic platform auto-detect (YouTube / Twitter-X / Instagram / Facebook)
- Preview (thumbnail + title) before download -- the key UX confirmation safeguard
- MP4 download with quality selection (Best / 1080p / 720p / 480p / 360p; default 720p)
- MP3 audio extraction via ffmpeg (fixed 192 kbps acceptable for v1)
- Visible, multi-stage-aware download progress (percent, speed, ETA) with an indeterminate "Finalizing..." state for the ffmpeg merge
- Cancel/stop that cleanly kills the entire process tree and removes partial .part files
- Open output folder / reveal file after download
- Clear success and failure states -- never a bar stuck at 100% with no message
- Human-readable error messages mapped from cryptic yt-dlp stderr
- Bundled engine (zero external dependencies) and no-admin per-user install
- Responsive UI throughout (no frozen window)
- **yt-dlp self-update** -- a hidden but near-mandatory table stake; a frozen engine breaks in 1-3 months

Recommended cheap v1 polish wins: MP3 bitrate dropdown (128/192/320), thumbnail-embed album art, remember last format/quality.

## Architecture in One Page

**Components** -- single solution, no DI container, no MVVM framework, 7 logical components split into UI-free Core/Process layers for testability:

- MainForm -- UI rendering only; never spawns processes or parses output
- DownloadCoordinator -- orchestrates a job, owns the single CancellationTokenSource, exposes a clean async API to the form
- MetadataFetcher -- yt-dlp --dump-single-json, deserializes VideoMetadata, enforces a ~20-30s timeout
- DownloadEngine + ProgressParser -- builds yt-dlp args, parses progress into DownloadProgress
- ThumbnailService -- shared static HttpClient, decodes image, placeholder fallback
- YtDlpProcessRunner -- the only component touching System.Diagnostics.Process
- ToolLocator -- resolves yt-dlp.exe/ffmpeg.exe from AppContext.BaseDirectory, verifies at startup

**Data flow:** URL pasted (debounced ~500ms) -> metadata fetch (--dump-single-json --no-playlist) -> VideoMetadata -> concurrent thumbnail fetch into PictureBox -> user picks MP3/MP4 + quality -> DownloadEngine builds args -> YtDlpProcessRunner runs yt-dlp with --newline progress (ffmpeg merges internally) -> stdout parsed -> IProgress<DownloadProgress> marshals to UI thread -> exit 0 writes file to %USERPROFILE%/Downloads/VidDrop/; non-zero exit -> ErrorClassifier -> friendly inline error. Metadata fetch and download are two separate yt-dlp invocations.

**Threading:** Task + async/await with IProgress<T> -- not BackgroundWorker. Event handlers are async void (sanctioned). new Progress<DownloadProgress>(...) captures the UI SynchronizationContext, so progress callbacks run on the UI thread with no manual Control.Invoke. Use .ConfigureAwait(false) in Core/Process code. One job at a time -- single CTS, disable Download while running, kill the process tree on FormClosing.

## Critical Pitfalls to Avoid

Top 5 ranked by risk:

1. **Antivirus false positives on yt-dlp.exe** (PyInstaller builds get flagged by Defender/AV) -- code-sign installer and app (EV cert ~200-400 USD/yr), pre-submit binaries to AV vendors; consider first-run download from GitHub instead of bundling.
2. **Frozen yt-dlp goes stale** (YouTube/IG/X change formats; breaks in 1-3 months) -- ship yt-dlp -U self-update on the writable per-user install; treat as permanent maintenance, not a one-time feature.
3. **stdout/stderr pipe deadlock** (synchronous reads on both streams hang when a buffer fills) -- always use async BeginOutputReadLine() + BeginErrorReadLine(), never ReadToEnd() on both.
4. **Zombie ffmpeg on cancel** (Kill() orphans the ffmpeg grandchild) -- use Kill(entireProcessTree: true), then await WaitForExitAsync, then delete partial files.
5. **UTF-8 title corruption** (Windows OEM codepage garbles non-ASCII titles and filenames) -- explicitly set StandardOutputEncoding/StandardErrorEncoding = Encoding.UTF8 before starting the process.

Also high-priority: pass --ffmpeg-location from AppContext.BaseDirectory or MP3/merge fails silently (C5); use ArgumentList not a shell string to avoid URL ampersand injection bugs (C7); use AppContext.BaseDirectory not Assembly.Location for single-file publish (M10).

## Phase Build Order

1. **Engine core: YtDlpProcessRunner + ToolLocator** (no UI) -- test via console harness. Highest-risk slice: solve pipe deadlock, zombie ffmpeg, UTF-8 encoding, URL injection, path resolution here once.
2. **MetadataFetcher + VideoMetadata model** -- console-test YouTube, Twitter, and a bad URL; enforce probe timeout. Depends on P1.
3. **DownloadEngine + ProgressParser + DownloadOptions** -- download a short MP4 and MP3, verify multi-stage progress parsing and ffmpeg merge, filename sanitization. Depends on P1.
4. **DownloadCoordinator + minimal MainForm** -- URL box, Fetch, format/quality combos, Download, progress bar, cancel. Depends on P2 + P3.
5. **ThumbnailService + preview polish** -- PictureBox, loading spinner, GDI disposal. Depends on P4.
6. **Error handling + robustness** -- ErrorClassifier, inline (non-modal) error panel, cancel UX, partial-file cleanup, "Done" states. Depends on P5.
7. **XP aesthetic + Inno Setup packaging** -- classic-gray control styling, icon, bundle yt-dlp + ffmpeg, code signing, per-user installer, self-update wiring. Depends on P6.

Dependency graph: P1 -> P2 and P1 -> P3; (P2 + P3) -> P4 -> P5 -> P6 -> P7.

**Research flags:** Phases 1 and 7 warrant deeper research during planning -- Phase 1 for current yt-dlp process/flag behavior and the bundle-vs-first-run-download decision, Phase 7 for code signing logistics and Inno Setup specifics. Phases 2-6 follow well-documented .NET patterns and can likely skip dedicated research.

## Open Questions

Verify before or during implementation:

1. **Current LTS .NET version** and real self-contained single-file publish size (research assumed .NET 8).
2. **YoutubeDLSharp** NuGet version, project health, and exact API method names (RunVideoDownload/RunAudioDownload/RunVideoDataFetch).
3. **Latest yt-dlp release** and the chosen update strategy -- yt-dlp -U self-update vs on-demand GitHub download vs bundle-only.
4. **ffmpeg build source and license** -- confirm LGPL (not GPL) variant; include LICENSES.txt.
5. **Latest Inno Setup 6.x** version.
6. **Stakeholder definition of XP aesthetic** -- cheap classic-gray (remove Application.EnableVisualStyles()) vs expensive owner-drawn Luna-blue. This materially affects Phase 7 effort.
7. **Bundle vs first-run-download for yt-dlp.exe** -- first-run download sidesteps both AV false positives and staleness but adds a network dependency and offline-install friction; decide in the architecture phase.
8. **ErrorClassifier stderr patterns** -- verify against current yt-dlp stderr output, as message strings change between releases.
9. **YouTube 1080p+ scope** -- DASH always forces a two-phase video+audio merge; confirm whether v1 supports 1080p+ or caps at 720p for simplicity.
10. **Code signing certificate** -- budget and procurement for an EV cert (eliminates SmartScreen immediately; ~200-400 USD/year).

## Confidence Assessment

| Area | Confidence | Notes |
|------|------------|-------|
| Stack | MEDIUM | Patterns sound; exact version numbers unverified (LOW) |
| Features | MEDIUM | Table stakes clear; drawn from domain norms without web verification |
| Architecture | HIGH | .NET/WinForms/process patterns are well-established |
| Pitfalls | MEDIUM-HIGH | Process-management pitfalls high confidence; exact yt-dlp flags/stderr need verification |

*Overall: MEDIUM. The build approach is solid; remaining risk is concentrated in unverified version numbers, current yt-dlp flag/stderr behavior, and the stakeholder/signing decisions called out above.*

---
*Synthesized from STACK.md, FEATURES.md, ARCHITECTURE.md, PITFALLS.md -- 2026-05-18*
