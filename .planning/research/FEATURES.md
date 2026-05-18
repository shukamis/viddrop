# Features Research: VidDrop

**Researched:** 2026-05-18 | **Overall confidence:** MEDIUM (web tools unavailable)

## Table Stakes
(must-have or users leave)

| Feature | Notes | Complexity |
|---|---|---|
| Paste URL + auto-detect platform | Auto-detect YouTube/Instagram/etc from URL | Quick |
| Preview (thumbnail + title) before download | Confirm correct video before downloading | Medium |
| MP4 download with quality selection | 1080p / 720p / 480p / 360p / Best | Medium |
| MP3 extraction | Audio-only via ffmpeg | Medium |
| Visible download progress | %, speed, ETA | Medium |
| Cancel/stop download | Kill child process cleanly | Medium |
| Open output folder after download | Reveal file in Explorer | Quick |
| Clear success/failure state | User knows when done | Quick |
| Human-readable error messages | Map yt-dlp stderr to plain language | Medium |
| Bundled engine (zero dependencies) | yt-dlp + ffmpeg in installer | Medium |
| No-admin install | Per-user install, no UAC | Quick |
| Responsive UI during download | No frozen window | Medium |
| **yt-dlp engine self-update** | CRITICAL: frozen yt-dlp breaks in 1-3 months | Medium |

> **Critical hidden table stake:** yt-dlp breaks frequently as YouTube/Instagram/etc change their APIs. A frozen bundled version self-destructs post-launch. Self-update (or at minimum update-check + easy manual update) must be treated as near-mandatory for v1.

## Differentiators
(defer most to v2, unless marked Quick)

| Feature | v1/v2 | Complexity |
|---|---|---|
| MP3 bitrate choice (128/192/320 kbps) | v1 candidate | Quick |
| Thumbnail embed in MP3 (album art) | v1 candidate | Quick |
| Drag-and-drop URL onto window | v2 | Quick |
| Download queue (multiple URLs) | v2 | Hard |
| Playlist support | v2 | Hard |
| Subtitles/captions download | v2 | Medium |
| Trim/clip (start-end time) | v2 | Hard |
| Cookie import for age-restricted content | v2 | Medium |
| Filesize estimate before download | v2 | Quick |
| Remember last format/quality choice | v1 polish | Quick |
| Change output folder | v2 | Quick |
| Smart auto-update with changelog | v2 | Medium |

**Recommended v1 polish additions:** MP3 bitrate dropdown + thumbnail embed (both Quick wins).

## Anti-Features
(deliberate exclusions — document the reason to prevent scope creep)

| Anti-Feature | Reason |
|---|---|
| Installer adware / bundled offers | Destroys trust instantly |
| Persistent download history DB | Already excluded — session-only |
| In-app credential storage | Security liability |
| Aggressive page scraping | Triggers rate-limiting and bans |
| In-app player / converter suite | Scope explosion, not the core job |
| Exposing raw yt-dlp flags to all users | Confuses non-technical users |
| Auto-download without preview confirmation | Removes the key UX safeguard |
| Silent phone-home analytics | Privacy concern, no consent |
| DRM circumvention tools | Legal liability |
| Shipping a never-updated yt-dlp | App self-destructs post-launch |
| Unbounded parallel downloads | Triggers platform rate-limiting |
| Continuous background clipboard polling | Invasive, annoying on first use |

## UX Patterns

### Quality Selection
- ComboBox of fixed presets: Best / 1080p / 720p / 480p / 360p
- Populated after the metadata probe (not before)
- Default: 720p
- Unavailable options disabled with annotation (e.g. "1080p — not available")
- Two-control pattern (Format + Quality) is the ecosystem norm

### Format Selection
- Radio buttons or segmented toggle: **MP4** | **MP3**
- Placed above the quality control
- Quality control adapts: MP3 shows bitrate dropdown, MP4 shows resolution
- v1 can ship MP3 at fixed 192 kbps (add bitrate choice later)

### Progress Display
- Determinate ProgressBar from yt-dlp stdout
- Use `--newline --progress-template` for stable parsing (not default format)
- Show: percentage, speed (MB/s), ETA
- **Three-stage reality:** video download → audio download → ffmpeg merge
- Merge stage has no % — show indeterminate "Finalizing..." state
- Never leave bar stuck at 100% without a completion message
- Probe stage: indeterminate spinner with "Fetching info..."

### Error Handling
The biggest differentiator — map cryptic yt-dlp stderr to plain messages:

| yt-dlp Error Pattern | User-Facing Message | Action |
|---|---|---|
| `Unsupported URL` | "This URL isn't supported. Try YouTube, Instagram, Twitter, or Facebook." | — |
| `HTTP Error 429` | "Too many requests. Wait a few minutes and try again." | — |
| `HTTP Error 403` | "Access denied. This content may be private or geo-restricted." | — |
| `This video is age-restricted` | "This video requires sign-in to download." | Show cookie import option (v2) |
| `Video unavailable` | "This video is unavailable or has been deleted." | — |
| `ffmpeg not found` | "Internal error: ffmpeg missing. Try reinstalling the app." | Reinstall link |
| `No such format` | "Requested quality not available. Try a lower resolution." | — |
| `[download] Destination:` (permission) | "Can't write to download folder. Check permissions." | Open folder button |
| Network timeout | "Connection timed out. Check your internet and try again." | Retry button |
| Non-zero exit, unknown | "Download failed. [Details ▼]" + raw stderr in expander | — |

**Three error categories:**
- **Your fault:** bad URL, private content
- **Our fault:** engine bug, missing binary
- **Site's fault:** rate limit, format change

## Platform Quirks

### YouTube
- Most reliable extractor
- Needs frequent yt-dlp engine updates
- 1080p+ requires DASH: separate video+audio streams merged by ffmpeg (two-phase progress)
- Age-restricted: needs `--cookies-from-browser` (v2)
- Rate limiting on rapid repeated requests

### Twitter/X
- Most volatile extractor — most likely to break first
- API changes frequently
- Expect first breakage within 1-3 months of launch
- Videos are often short, no quality variants

### Instagram
- Heavy 429 rate-limiting
- Login walls for non-public content
- Never do rapid/parallel requests (triggers shadow-ban of the IP)
- Reels and posts work; Stories require login

### Facebook
- Public videos work reliably
- Private/friends-only videos don't — be clear in error message
- URL detection: many Facebook URL formats exist, be permissive in detection

### Generic URLs
- Best-effort only
- Gate via the metadata probe — if probe fails, show "Format not supported" before offering download

## Feature Complexity Map

| Feature | Complexity | v1/v2 |
|---|---|---|
| URL paste + platform auto-detect | Quick | v1 |
| Metadata probe (title/thumbnail) | Medium | v1 |
| MP4 download + quality selection | Medium | v1 |
| MP3 extraction | Medium | v1 |
| Progress bar (3-stage aware) | Medium | v1 |
| Cancel download | Medium | v1 |
| Error message mapping | Medium | v1 |
| Open output folder button | Quick | v1 |
| yt-dlp self-update | Medium | v1 |
| MP3 bitrate selection | Quick | v1 candidate |
| Thumbnail embed in MP3 | Quick | v1 candidate |
| Remember last format/quality | Quick | v1 polish |
| Drag-and-drop URL | Quick | v2 |
| Change output folder | Quick | v2 |
| Playlist support | Hard | v2 |
| Download queue | Hard | v2 |
| Cookie import | Medium | v2 |
| Subtitles | Medium | v2 |
| Trim/clip | Hard | v2 |

## Roadmap Implications

1. Build the yt-dlp child-process integration first — everything depends on it
2. Error-message mapping deserves its own phase slice — it defines perceived quality
3. Budget real time for multi-stage progress UX (video + audio + merge)
4. yt-dlp self-update is a permanent maintenance condition, not a one-time feature
5. Twitter/X extractor: set user expectations early (may break between releases)

---
*Research complete: 2026-05-18*
