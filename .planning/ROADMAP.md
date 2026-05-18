# VidDrop - Roadmap

**Created:** 2026-05-18
**Mode:** mvp (Vertical MVP - each phase delivers a working end-to-end user capability)
**Granularity:** fine
**Coverage:** 27/27 v1 requirements mapped

## Phases

- [ ] **Phase 1: First Download (YouTube MP4)** - User pastes a YouTube URL and downloads it as an MP4.
- [ ] **Phase 2: Audio & Quality Choices** - User picks MP3/MP4, video quality, and MP3 bitrate.
- [ ] **Phase 3: Preview Before Download** - User sees thumbnail + title before committing to a download.
- [ ] **Phase 4: All Platforms** - User downloads from Twitter/X, Instagram, and Facebook, not just YouTube.
- [ ] **Phase 5: Robust Progress & Cancel** - User sees multi-stage progress, can cancel cleanly, and reads friendly errors.
- [ ] **Phase 6: Frutiger Aero Look & Installer** - User installs a polished, glossy app via a Windows installer.
- [ ] **Phase 7: Auto-Cut (Viral Clip Generator)** - User generates short viral clips from long videos using AI.

## Phase Details

### Phase 1: First Download (YouTube MP4)
**Goal:** User can paste a YouTube URL into VidDrop and download the video as an MP4 file to their Downloads folder.
**Mode:** mvp
**Depends on:** Nothing (first phase)
**Requirements:** DL-01, PLAT-01, ENG-03, ENG-04
**Success Criteria:**
  1. User can paste a YouTube URL, click Download, and an MP4 file appears in %USERPROFILE%\Downloads\VidDrop.
  2. On startup, if yt-dlp.exe or ffmpeg.exe is missing, the user sees a clear message instead of a crash.
  3. The VidDrop output folder is created automatically the first time it is needed.
  4. The downloaded MP4 plays correctly with video and audio merged.
**Plans:** TBD
**UI hint:** yes

### Phase 2: Audio & Quality Choices
**Goal:** User can choose between MP4 and MP3, select video quality (Best/1080p/720p/480p/360p), and pick MP3 bitrate (128/192/320), getting MP3 files with embedded album art.
**Mode:** mvp
**Depends on:** Phase 1
**Requirements:** DL-02, DL-03, DL-04
**Success Criteria:**
  1. User can select MP3 and download an audio-only file extracted from a YouTube video.
  2. User can choose a video quality from a dropdown and the resulting MP4 matches that resolution.
  3. User can choose an MP3 bitrate (128/192/320 kbps) and the downloaded file uses it.
  4. A downloaded MP3 shows the video thumbnail as album art in a media player.
**Plans:** TBD
**UI hint:** yes

### Phase 3: Preview Before Download
**Goal:** User sees the video thumbnail and title automatically after pasting a URL, confirming it is the right content before downloading.
**Mode:** mvp
**Depends on:** Phase 2
**Requirements:** UX-01
**Success Criteria:**
  1. After pasting a URL, the thumbnail and title of the video appear without the user clicking anything extra.
  2. While metadata is loading, the user sees a loading indicator rather than a frozen window.
  3. If metadata cannot be fetched, the preview area shows a clear placeholder instead of a broken image.
**Plans:** TBD
**UI hint:** yes

### Phase 4: All Platforms
**Goal:** User can download from Twitter/X, Instagram, and Facebook URLs, with the app auto-detecting the platform from any pasted URL.
**Mode:** mvp
**Depends on:** Phase 3
**Requirements:** PLAT-02, PLAT-03, PLAT-04, PLAT-05
**Success Criteria:**
  1. User can paste a Twitter/X URL and download the video successfully.
  2. User can paste an Instagram URL and download the video successfully.
  3. User can paste a Facebook URL and download the video successfully.
  4. The app recognizes the platform from the pasted URL with no manual platform selection.
**Plans:** TBD
**UI hint:** no

### Phase 5: Robust Progress & Cancel
**Goal:** User sees accurate multi-stage progress, a clear success state, friendly error messages, and can cancel a download cleanly without leftover files.
**Mode:** mvp
**Depends on:** Phase 4
**Requirements:** UX-02, UX-03, UX-04, ENG-02
**Success Criteria:**
  1. During a download the user sees distinct stages (Video % / Audio % / Finalizing...) rather than one ambiguous bar.
  2. User can click Cancel mid-download; the process tree stops and partial files are removed.
  3. After a successful download the user sees a clear Done state, never a bar stuck at 100%.
  4. When a download fails, the user sees a readable explanation instead of raw yt-dlp error text.
**Plans:** TBD
**UI hint:** yes

### Phase 6: Frutiger Aero Look & Installer
**Goal:** User can install VidDrop via a self-contained Windows installer and use a polished Frutiger Aero interface, with yt-dlp keeping itself up to date.
**Mode:** mvp
**Depends on:** Phase 5
**Requirements:** VIS-01, ENG-01, DIST-01, DIST-02
**Success Criteria:**
  1. The app shows a Frutiger Aero aesthetic (glossy panels, blue-teal gradients, rounded corners, glossy buttons).
  2. User can install VidDrop from a single .exe installer with no UAC prompt and no admin rights.
  3. The installed app runs with no external dependencies because yt-dlp and ffmpeg are bundled.
  4. yt-dlp updates itself so downloads keep working as platforms change.
**Plans:** TBD
**UI hint:** yes

### Phase 7: Auto-Cut (Viral Clip Generator)
**Goal:** User loads a video, selects an AI provider, and the app automatically generates short clips with captions and vertical format ready for TikTok/Reels/Shorts.
**Mode:** mvp
**Depends on:** Phase 6
**Requirements:** CLIP-01, CLIP-02, CLIP-03, CLIP-04, CLIP-05, CLIP-06, CLIP-07
**Success Criteria:**
  1. User can load a local video file or use a previously downloaded one for clip analysis.
  2. User can choose AI provider in settings: Whisper (local), OpenAI API, Gemini API, or Claude API.
  3. App transcribes the video and identifies high-engagement moments automatically.
  4. User sees a list of suggested clips and can approve or discard each one before export.
  5. Approved clips are exported with captions burned in and optionally converted to 9:16 vertical format.
**Plans:** TBD
**UI hint:** yes

## Progress

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. First Download (YouTube MP4) | 0/0 | Not started | - |
| 2. Audio & Quality Choices | 0/0 | Not started | - |
| 3. Preview Before Download | 0/0 | Not started | - |
| 4. All Platforms | 0/0 | Not started | - |
| 5. Robust Progress & Cancel | 0/0 | Not started | - |
| 6. Frutiger Aero Look & Installer | 0/0 | Not started | - |
| 7. Auto-Cut (Viral Clip Generator) | 0/0 | Not started | - |

---
*Roadmap created: 2026-05-18 | Last updated: 2026-05-18 (Phase 7 added)*

