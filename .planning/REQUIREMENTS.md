# Requirements: VidDrop

**Defined:** 2026-05-18
**Core Value:** Colar uma URL e ter o arquivo baixado em segundos, sem instalar mais nada alem do proprio app.

## v1 Requirements

### Download

- [ ] **DL-01**: Usuario pode baixar MP4 com selecao de qualidade (Best / 1080p / 720p / 480p / 360p)
- [ ] **DL-02**: Usuario pode extrair MP3 do audio do video
- [ ] **DL-03**: Usuario pode escolher o bitrate do MP3 (128 / 192 / 320 kbps)
- [ ] **DL-04**: MP3 baixado inclui thumbnail embutida como album art

### Plataformas

- [ ] **PLAT-01**: Download de URLs do YouTube
- [ ] **PLAT-02**: Download de URLs do Twitter/X
- [ ] **PLAT-03**: Download de URLs do Instagram
- [ ] **PLAT-04**: Download de URLs do Facebook
- [ ] **PLAT-05**: Auto-detecta plataforma a partir da URL colada

### Preview e UX

- [ ] **UX-01**: Usuario ve thumbnail e titulo do video antes de baixar
- [ ] **UX-02**: Barra de progresso multi-fase (Video % / Audio % / Finalizando...)
- [ ] **UX-03**: Botao cancelar que encerra processo e limpa arquivos parciais
- [ ] **UX-04**: Estado de sucesso claro apos download (sem barra travada em 100%)

### Engine e Infraestrutura

- [ ] **ENG-01**: yt-dlp se auto-atualiza para manter compatibilidade com as plataformas
- [ ] **ENG-02**: Erros cripticos do yt-dlp mapeados para mensagens legiveis ao usuario
- [ ] **ENG-03**: App verifica presenca de yt-dlp.exe e ffmpeg.exe na inicializacao
- [ ] **ENG-04**: App cria pasta de saida no primeiro uso (%USERPROFILE%\Downloads\VidDrop)

### Visual

- [ ] **VIS-01**: Interface com estetica Frutiger Aero (paineis glossy/vidrosos, gradientes azul-teal, bordas arredondadas, brilho nos botoes)

### Distribuicao

- [ ] **DIST-01**: Instalavel via installer Windows .exe (Inno Setup, per-user, sem UAC)
- [ ] **DIST-02**: Todas as dependencias embutidas no instalador (yt-dlp + ffmpeg)


### Auto-Cut (Phase 7)

- [ ] **CLIP-01**: Usuario pode carregar um video local ou usar um video ja baixado para analise
- [ ] **CLIP-02**: Usuario pode escolher o provedor de IA nas configuracoes (Whisper local, OpenAI API, Gemini API, Claude API)
- [ ] **CLIP-03**: App transcreve o audio do video e identifica momentos de alto engajamento automaticamente
- [ ] **CLIP-04**: Usuario ve lista de clips sugeridos e pode aprovar ou descartar cada um
- [ ] **CLIP-05**: Clips aprovados sao exportados com legendas (captions) queimadas no video
- [ ] **CLIP-06**: Usuario pode exportar clips no formato vertical 9:16 para TikTok/Reels/Shorts
- [ ] **CLIP-07**: App funciona offline com Whisper local e online com APIs (OpenAI, Gemini, Claude)

## v2 Requirements

### UX
- **UX-V2-01**: Abrir pasta de saida apos download
- **UX-V2-02**: Lembrar ultimo formato/qualidade entre sessoes
- **UX-V2-03**: Drag-and-drop de URL na janela

### Download
- **DL-V2-01**: Fila de multiplos downloads
- **DL-V2-02**: Suporte a playlists inteiras
- **DL-V2-03**: Import de cookies para conteudo com age-restriction
- **DL-V2-04**: Download de legendas/subtitles

## Out of Scope

| Feature | Razao |
|---|---|
| Historico de downloads persistente | Simplicidade - app de uso pontual |
| Login em contas (Instagram privado) | Complexidade e riscos de seguranca |
| App para Mac/Linux | Windows first |
| Player de video embutido | Nao e o trabalho do app |
| Conversao de formatos alem de MP3/MP4 | Fora do escopo core |

## Traceability

| Requirement | Phase | Status |
|---|---|---|
| DL-01 | Phase 1 | Pending |
| DL-02 | Phase 2 | Pending |
| DL-03 | Phase 2 | Pending |
| DL-04 | Phase 2 | Pending |
| PLAT-01 | Phase 1 | Pending |
| PLAT-02 | Phase 4 | Pending |
| PLAT-03 | Phase 4 | Pending |
| PLAT-04 | Phase 4 | Pending |
| PLAT-05 | Phase 4 | Pending |
| UX-01 | Phase 3 | Pending |
| UX-02 | Phase 5 | Pending |
| UX-03 | Phase 5 | Pending |
| UX-04 | Phase 5 | Pending |
| ENG-01 | Phase 6 | Pending |
| ENG-02 | Phase 5 | Pending |
| ENG-03 | Phase 1 | Pending |
| ENG-04 | Phase 1 | Pending |
| VIS-01 | Phase 6 | Pending |
| DIST-01 | Phase 6 | Pending |
| DIST-02 | Phase 6 | Pending |

| CLIP-01 | Phase 7 | Pending |
| CLIP-02 | Phase 7 | Pending |
| CLIP-03 | Phase 7 | Pending |
| CLIP-04 | Phase 7 | Pending |
| CLIP-05 | Phase 7 | Pending |
| CLIP-06 | Phase 7 | Pending |
| CLIP-07 | Phase 7 | Pending |

**Coverage:**
- v1 requirements: 27 total
- Mapped to phases: 20
- Unmapped: 0

---
*Requirements defined: 2026-05-18 | Traceability updated: 2026-05-18*

