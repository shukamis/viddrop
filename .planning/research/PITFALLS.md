# Pitfalls Research: VidDrop

**Researched:** 2026-05-18 | **Overall confidence:** MEDIUM (web tools unavailable)

## Critical Pitfalls
(show-stoppers — must address before shipping)

### C1: Antivirus False Positives
**Risk:** PyInstaller-packed `yt-dlp.exe` is routinely flagged by Windows Defender and third-party AV.
**Impact:** Users can't run the app. Download pages warn "dangerous file."
**Prevention:**
- Code-sign both the installer and the app executable (EV cert preferred)
- Pre-submit binaries to AV vendors via Microsoft Security Intelligence portal
- Consider downloading yt-dlp from official GitHub releases at first-run instead of bundling
**Phase:** Architecture phase — affects bundling strategy and budget.

### C2: stdout/stderr Pipe Deadlock
**Risk:** Reading both stdout and stderr synchronously from yt-dlp's Process will deadlock when a pipe buffer fills.
**Prevention:** Always use `BeginOutputReadLine()` + `BeginErrorReadLine()` (async event-driven reads). Never `ReadToEnd()` synchronously on both streams.
**Phase:** Engine integration phase.

### C3: Zombie ffmpeg Process on Cancel
**Risk:** `process.Kill()` kills yt-dlp but leaves an orphaned ffmpeg child process consuming CPU/disk.
**Prevention:** Use `process.Kill(entireProcessTree: true)` (.NET 5+). Also delete incomplete output files on cancel.
**Phase:** Engine integration phase.

### C4: UTF-8 Title Corruption
**Risk:** Windows defaults to OEM codepage for child process stdout. Non-ASCII video titles (Japanese, Arabic, emoji) become garbage characters and corrupt filenames.
**Prevention:** Explicitly set `StartInfo.StandardOutputEncoding = Encoding.UTF8` and `StandardErrorEncoding = Encoding.UTF8` before starting the process.
**Phase:** Engine integration phase.

### C5: ffmpeg Not Found Silently
**Risk:** If yt-dlp can't find ffmpeg, MP3 extraction fails silently or produces video-only MP4 without audio merge. No error visible to user.
**Prevention:** Always pass `--ffmpeg-location` resolved from `AppContext.BaseDirectory`. Verify ffmpeg.exe exists at startup and show a clear reinstall prompt if missing.
**Phase:** Engine integration + installer phase.

### C6: Frozen yt-dlp Goes Stale
**Risk:** YouTube, Instagram, Twitter/X change their formats/APIs frequently. A frozen bundled yt-dlp.exe breaks within 1-3 months.
**Impact:** App stops working for all users. Requires new installer release.
**Prevention:** Implement `yt-dlp -U` self-update or on-demand update from GitHub releases. Per-user install (`%LOCALAPPDATA%`) is required so the folder is writable without UAC.
**Phase:** Must be planned in architecture phase.

### C7: URL Argument Injection
**Risk:** URLs containing `&` characters (YouTube playlists, query params) break when passed through `cmd.exe` shell. Also risk of argument injection if URL is user-supplied.
**Prevention:** Use `Process.StartInfo.ArgumentList` (not `Arguments` string) — each argument is a separate entry, never passed through a shell. Never use `cmd /c yt-dlp ...`.
**Phase:** Engine integration phase.

## Common Mistakes
(frequent bugs with prevention strategies)

### M1: Two-Phase Progress Confusion
**Mistake:** Showing a single 0-100% bar that resets to 0% when the audio download starts (after video).
**Fix:** Track download phase state (video / audio / merging). Show "Downloading video... X%" then "Downloading audio... X%" then indeterminate "Finalizing...".

### M2: Progress Bar Stuck at 100%
**Mistake:** Bar hits 100% but ffmpeg merge takes 10-30s. UI looks frozen.
**Fix:** Switch to indeterminate mode during merge stage. Show "Finalizing..." label.

### M3: Quality Not Available
**Mistake:** User selects 1080p, it fails silently or errors cryptically because the video only has 720p.
**Fix:** Disable unavailable quality options in the dropdown after probe. Use `--format bestvideo[height<=N]+bestaudio/best[height<=N]` fallback.

### M4: Filename Illegal Characters
**Mistake:** Video titles with `\/:*?"<>|` characters crash the download with a file system error.
**Fix:** Sanitize via `--restrict-filenames` flag, or apply a custom sanitization function. Test with titles containing colons, slashes, emojis.

### M5: No Cancel Button
**Mistake:** Starting a download with no way to stop it. Process keeps running even if user closes the window.
**Fix:** Cancel button from the start. Handle FormClosing event to kill child processes.

### M6: Thumbnail Fetch Blocks UI
**Mistake:** Downloading the thumbnail image on the UI thread freezes the window.
**Fix:** Use `async/await` with `HttpClient.GetByteArrayAsync()`. Display placeholder while loading.

### M7: Multiple Simultaneous Downloads
**Mistake:** User clicks Download while one is already running — starts a second process, both writing to same filename.
**Fix:** Disable the Download button while a download is active, or implement a proper queue.

### M8: Probe Timeout Not Handled
**Mistake:** yt-dlp hangs on probe (bad URL, network issue) — UI spins forever.
**Fix:** Set a timeout (15-30s) on the metadata probe. Show "Taking too long — check the URL" if exceeded.

### M9: Error from Wrong Stream
**Mistake:** Reading only stdout for errors. yt-dlp writes most errors to stderr.
**Fix:** Always capture both streams. Prioritize stderr for error message display.

### M10: AppContext.BaseDirectory vs Assembly.Location
**Mistake:** Using `Assembly.Location` to find bundled binaries fails in .NET single-file publish — it returns empty string.
**Fix:** Always use `AppContext.BaseDirectory` for locating yt-dlp.exe and ffmpeg.exe.

### M11: Output Folder Not Created
**Mistake:** Assuming `%USERPROFILE%\Downloads\VidDrop` exists — it doesn't until explicitly created.
**Fix:** Call `Directory.CreateDirectory(outputPath)` at app startup (idempotent).

## Platform-Specific Risks

### YouTube
- Most stable but breaks most often (largest target for anti-scraping)
- 1080p+ always requires DASH merge (two-phase progress) — handle or limit to 720p max for simplicity
- Age-restricted content requires cookies — graceful "sign-in required" message
- Shorts URLs (`/shorts/xxx`) work but need testing

### Twitter/X
- Most volatile extractor — expect breakage first
- API frequently changes (v2 API shift, authentication changes)
- Mitigation: communicate to users that Twitter support may lag between updates
- Videos often have no quality variants (one format only)

### Instagram
- Rate limiting (HTTP 429) triggers on >3-5 requests per minute from same IP
- Login required for non-public content (Reels from private accounts)
- Never request metadata twice rapidly (debounce URL input)
- Stories: require login — out of scope for v1

### Facebook
- Public videos work; private don't
- Many URL formats: `/watch?v=`, `/reel/`, `/videos/` — test all patterns
- `m.facebook.com` URLs may differ from `www.facebook.com`

## Distribution Risks

### D1: Code Signing
Without a code signing certificate, Windows SmartScreen shows "Unknown Publisher" warning. Users must click through. EV cert eliminates SmartScreen immediately; standard OV cert reduces warnings over time.
**Budget impact:** EV cert ~$200-400/year.

### D2: Inno Setup Per-User Install
Use `PrivilegesRequired=lowest` and `DefaultDirName={localappdata}\Programs\VidDrop`. This avoids UAC, allows yt-dlp self-update (folder is writable), and works for standard user accounts.

### D3: .NET Runtime
Self-contained publish (publish profile: `win-x64`, `SelfContained=true`, `PublishSingleFile=true`) includes the runtime. Framework-dependent builds will fail on fresh Windows installs.

### D4: Installer Size
Self-contained .NET 8 + ffmpeg essentials + yt-dlp ≈ 80-100 MB installer. This is expected and acceptable. Use `lzma2/ultra64` compression in Inno Setup.

### D5: ffmpeg License
Use an **LGPL build** of ffmpeg (not GPL). Include `LICENSES.txt` in the installer with attribution. GPL build imposes stronger redistribution requirements.

### D6: yt-dlp License
yt-dlp is Unlicense. No restrictions on bundling. Attribution appreciated but not required.

### D7: Platform ToS
Downloading content may violate YouTube/Instagram/Facebook Terms of Service in certain uses. This is a known risk for all video downloader apps. Scope the app for personal/fair-use scenarios.

### D8: Antivirus (see C1)
Plan for AV false positives from day one. Code signing is the primary mitigation.

## UX Pitfalls

- **U1:** No feedback when URL is invalid — user wonders if app is broken
- **U2:** Progress % disappears on completion without a "Done" state — confusing
- **U3:** Download button active before probe completes — race condition
- **U4:** Output filename shown nowhere — user doesn't know what was saved
- **U5:** No way to open the downloaded file directly — must navigate to folder
- **U6:** Error message shows raw yt-dlp stderr — unreadable to normal users
- **U7:** App crashes on second download attempt (resource not disposed)
- **U8:** Window resizable but UI breaks at small sizes — set MinimumSize
- **U9:** No loading indicator on thumbnail fetch — empty PictureBox looks broken
- **U10:** Quality dropdown shows "best" as a string — confusing; label it "Best available"
- **U11:** Cancel doesn't clean up partial files — leaves corrupt .part files in output folder
- **U12:** No "Update yt-dlp" indication when engine is outdated — silent degradation

## Phase Mapping

| Pitfall | Phase |
|---|---|
| stdout/stderr deadlock (C2) | Phase: Engine core |
| Zombie process on cancel (C3) | Phase: Engine core |
| UTF-8 encoding (C4) | Phase: Engine core |
| ffmpeg location (C5) | Phase: Engine core |
| URL argument injection (C7) | Phase: Engine core |
| Antivirus / code signing (C1) | Phase: Installer |
| yt-dlp self-update (C6) | Phase: Installer / Update |
| Two-phase progress (M1, M2) | Phase: UI / Progress |
| Quality availability (M3) | Phase: UI / Preview |
| Filename sanitization (M4) | Phase: Engine core |
| Cancel + cleanup (M5, U11) | Phase: Engine core |
| Probe timeout (M8) | Phase: UI / Preview |
| Output folder creation (M11) | Phase: Engine core |
| Error message mapping (M9, U6) | Phase: UI / Error handling |
| Code signing setup (D1) | Phase: Release / Installer |

---
*Research complete: 2026-05-18*
