# Discussion Log — Phase 1: First Download (YouTube MP4)

**Date:** 2026-05-18
**Mode:** default (interactive, user delegated all decisions mid-session)

## Areas Discussed

### yt-dlp wrapper
- Q: YoutubeDLSharp NuGet vs custom YtDlpProcessRunner?
  Selected: Custom YtDlpProcessRunner
- Q: Single project or separate VidDrop.Engine?
  Selected: Pasta Process/ no mesmo projeto
- Q: Progress parsing strategy?
  Selected: Claude decides → --progress-template determinístico

### UI da Phase 1
  Claude decided: TextBox + ComboBox qualidade + Download + ProgressBar + StatusLabel. FixedDialog 520x280.

### Fonte do yt-dlp
  Claude decided: Bundled em Tools/. AV deferred to Phase 6.

### Qualidade no Phase 1
  Claude decided: Implementar já — ComboBox Best/1080p/720p/480p/360p mapeado para -f selectors.

## Deferred Ideas

- YoutubeDLSharp como alternativa (rejeitado em favor de controle total)
- Projeto separado VidDrop.Engine (escopo pequeno não justifica)
