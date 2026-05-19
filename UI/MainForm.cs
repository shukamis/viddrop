using System.Drawing.Drawing2D;
using System.Drawing.Text;
using VidDrop.Core;
using VidDrop.Errors;
using VidDrop.Models;
using VidDrop.Process;

namespace VidDrop.UI;

public class MainForm : Form
{
    private readonly DownloadCoordinator _coordinator = new();
    private readonly MetadataFetcher _fetcher = new();
    private CancellationTokenSource? _fetchCts;
    private System.Threading.Timer? _debounce;
    private bool _isDownloading;

    // Controls
    private TextBox urlBox = null!;
    private PictureBox thumbBox = null!;
    private Label titleLabel = null!;
    private Label platformLabel = null!;
    private Label formatLabel = null!;
    private ComboBox formatCombo = null!;
    private Label qualityLabel = null!;
    private ComboBox qualityCombo = null!;
    private Label bitrateLabel = null!;
    private ComboBox bitrateCombo = null!;
    private AeroButton actionBtn = null!;
    private AeroProgressBar progressBar = null!;
    private Label statusLabel = null!;

    // Layout constants
    private const int CardLeft = 10;
    private const int CardTop  = 78;
    private const int CardRadius = 14;
    private const int InnerX = 26;    // CardLeft + 16
    private const int InnerW = 438;   // content width inside card

    public MainForm()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;
        InitializeComponent();
        VerifyToolsOnStartup();
        SelfUpdater.RunInBackground();
    }

    private void InitializeComponent()
    {
        Text = "VidDrop";
        ClientSize = new Size(490, 420);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);
        BackColor = Color.FromArgb(185, 220, 245); // fallback if painting fails

        // URL input
        urlBox = new TextBox
        {
            Location    = new Point(InnerX, 93),
            Size        = new Size(InnerW, 28),
            Font        = new Font("Segoe UI", 9.5f),
            PlaceholderText = "Cole uma URL (YouTube, Twitter/X, Instagram, Facebook...)",
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = Color.FromArgb(245, 251, 255),
        };
        urlBox.TextChanged += OnUrlChanged;

        // Platform badge
        platformLabel = new Label
        {
            Location  = new Point(InnerX, 127),
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
            BackColor = Color.Transparent,
            Visible   = false,
        };

        // Thumbnail
        thumbBox = new PictureBox
        {
            Location    = new Point(InnerX, 148),
            Size        = new Size(112, 68),
            SizeMode    = PictureBoxSizeMode.Zoom,
            BackColor   = Color.FromArgb(200, 228, 248),
            BorderStyle = BorderStyle.FixedSingle,
        };

        // Video title
        titleLabel = new Label
        {
            Location  = new Point(InnerX + 122, 148),
            Size      = new Size(InnerW - 122, 68),
            Text      = "Cole uma URL acima",
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(30, 55, 90),
            Font      = new Font("Segoe UI", 9.5f),
            AutoEllipsis = true,
        };

        // Format row
        formatLabel = new Label
        {
            Location  = new Point(InnerX, 232),
            AutoSize  = true,
            Text      = "Formato:",
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(40, 65, 100),
        };

        formatCombo = new ComboBox
        {
            Location      = new Point(InnerX + 62, 228),
            Size          = new Size(76, 26),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9f),
        };
        formatCombo.Items.AddRange(["MP4", "MP3"]);
        formatCombo.SelectedIndex = 0;
        formatCombo.SelectedIndexChanged += OnFormatChanged;

        qualityLabel = new Label
        {
            Location  = new Point(InnerX + 154, 232),
            AutoSize  = true,
            Text      = "Qualidade:",
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(40, 65, 100),
        };

        qualityCombo = new ComboBox
        {
            Location      = new Point(InnerX + 224, 228),
            Size          = new Size(100, 26),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9f),
        };
        qualityCombo.Items.AddRange(["Melhor", "1080p", "720p", "480p", "360p"]);
        qualityCombo.SelectedIndex = 0;

        bitrateLabel = new Label
        {
            Location  = new Point(InnerX + 154, 232),
            AutoSize  = true,
            Text      = "Bitrate:",
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(40, 65, 100),
            Visible   = false,
        };

        bitrateCombo = new ComboBox
        {
            Location      = new Point(InnerX + 210, 228),
            Size          = new Size(96, 26),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9f),
            Visible       = false,
        };
        bitrateCombo.Items.AddRange(["128 kbps", "192 kbps", "320 kbps"]);
        bitrateCombo.SelectedIndex = 1;

        // Action button
        actionBtn = new AeroButton
        {
            Location = new Point(InnerX, 268),
            Size     = new Size(InnerW, 40),
            Text     = "Baixar",
        };
        actionBtn.Click += OnActionClick;

        // Progress bar
        progressBar = new AeroProgressBar
        {
            Location = new Point(InnerX, 322),
            Size     = new Size(InnerW, 22),
            Maximum  = 100,
            Value    = 0,
            Visible  = false,
        };

        // Status label
        statusLabel = new Label
        {
            Location  = new Point(InnerX, 354),
            Size      = new Size(InnerW, 44),
            Text      = "Cole uma URL e clique em Baixar",
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(45, 75, 115),
            Font      = new Font("Segoe UI", 8.5f),
            AutoEllipsis = true,
        };

        Controls.AddRange([
            urlBox,
            platformLabel,
            thumbBox, titleLabel,
            formatLabel, formatCombo,
            qualityLabel, qualityCombo,
            bitrateLabel, bitrateCombo,
            actionBtn,
            progressBar,
            statusLabel,
        ]);
    }

    // ── Painting ─────────────────────────────────────────────────────────────

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // Sky gradient
        using (var sky = new LinearGradientBrush(
            new Point(0, 0), new Point(0, Height),
            Color.FromArgb(228, 244, 255),
            Color.FromArgb(100, 172, 220)))
        {
            g.FillRectangle(sky, ClientRectangle);
        }

        // Soft radial light orb — upper-right
        using (var orbPath = new GraphicsPath())
        {
            orbPath.AddEllipse(Width - 160, -120, 380, 340);
            using var orb = new PathGradientBrush(orbPath);
            orb.CenterColor    = Color.FromArgb(95, 255, 255, 255);
            orb.SurroundColors = [Color.FromArgb(0, 255, 255, 255)];
            g.FillPath(orb, orbPath);
        }

        // VidDrop icon orb
        DrawIconOrb(g, 18, 14, 42);

        // Title
        using var titleFont = new Font("Segoe UI", 20f, FontStyle.Bold);
        TextRenderer.DrawText(g, "VidDrop", titleFont, new Point(69, 18),
            Color.FromArgb(55, 0, 70, 130));                    // shadow
        TextRenderer.DrawText(g, "VidDrop", titleFont, new Point(68, 16),
            Color.White);

        // Tagline
        using var tagFont = new Font("Segoe UI", 8.5f, FontStyle.Italic);
        TextRenderer.DrawText(g, "Baixe qualquer vídeo — rápido e fácil", tagFont,
            new Point(70, 50), Color.FromArgb(210, 255, 255, 255));

        // Glass card drop shadow
        var shadowCard = new Rectangle(CardLeft + 2, CardTop + 4, Width - CardLeft * 2, Height - CardTop - 12);
        using (var sp = RoundedPath(shadowCard, CardRadius))
        using (var sb = new SolidBrush(Color.FromArgb(38, 0, 55, 120)))
            g.FillPath(sb, sp);

        // Glass card fill
        var card = new Rectangle(CardLeft, CardTop, Width - CardLeft * 2, Height - CardTop - 12);
        using var cardPath = RoundedPath(card, CardRadius);
        using (var cf = new SolidBrush(Color.FromArgb(218, 248, 253, 255)))
            g.FillPath(cf, cardPath);

        // Card top-gloss (upper 35%)
        int glossH = (int)((Height - CardTop - 12) * 0.35f);
        var glossRect = new Rectangle(CardLeft + 1, CardTop + 1, Width - CardLeft * 2 - 2, glossH);
        using (var gp = RoundedPath(glossRect, CardRadius - 1))
        using (var gb = new LinearGradientBrush(
            new Point(0, glossRect.Top), new Point(0, glossRect.Bottom),
            Color.FromArgb(195, 255, 255, 255),
            Color.FromArgb(0, 255, 255, 255)))
            g.FillPath(gb, gp);

        // Card bright inner border
        using (var bp = new Pen(Color.FromArgb(205, 255, 255, 255), 1.5f))
            g.DrawPath(bp, cardPath);

        // Card outer hairline
        var outerRect = new Rectangle(CardLeft + 1, CardTop + 1, Width - CardLeft * 2 - 2, Height - CardTop - 14);
        using (var op = RoundedPath(outerRect, CardRadius - 1))
        using (var ob = new Pen(Color.FromArgb(55, 80, 135, 185), 1f))
            g.DrawPath(ob, op);
    }

    private static void DrawIconOrb(Graphics g, int x, int y, int size)
    {
        var r = new Rectangle(x, y, size, size);

        // Outer glow
        var glowR = new Rectangle(x - 3, y - 3, size + 6, size + 6);
        using (var glowPath = new GraphicsPath())
        {
            glowPath.AddEllipse(glowR);
            using var pgb = new PathGradientBrush(glowPath);
            pgb.CenterColor    = Color.FromArgb(80, 60, 180, 255);
            pgb.SurroundColors = [Color.FromArgb(0, 60, 180, 255)];
            g.FillPath(pgb, glowPath);
        }

        // Main orb gradient
        using (var bg = new LinearGradientBrush(r,
            Color.FromArgb(255, 80, 195, 255),
            Color.FromArgb(255, 10, 100, 210), 90f))
            g.FillEllipse(bg, r);

        // Play triangle
        int cx = x + size / 2 + 2, cy = y + size / 2;
        int th = size / 3;
        var tri = new Point[]
        {
            new(cx - th / 2, cy - th),
            new(cx - th / 2, cy + th),
            new(cx + th,     cy),
        };
        using var wb = new SolidBrush(Color.FromArgb(230, 255, 255, 255));
        g.FillPolygon(wb, tri);

        // Orb top gloss
        var glossE = new Rectangle(x + 2, y + 2, size - 4, size / 2);
        using var gBrush = new LinearGradientBrush(glossE,
            Color.FromArgb(170, 255, 255, 255),
            Color.FromArgb(0, 255, 255, 255), 90f);
        g.FillEllipse(gBrush, glossE);
    }

    private static GraphicsPath RoundedPath(Rectangle rect, int r)
    {
        int d = r * 2;
        var p = new GraphicsPath();
        p.AddArc(rect.X,          rect.Y,          d, d, 180, 90);
        p.AddArc(rect.Right - d,  rect.Y,          d, d, 270, 90);
        p.AddArc(rect.Right - d,  rect.Bottom - d, d, d,   0, 90);
        p.AddArc(rect.X,          rect.Bottom - d, d, d,  90, 90);
        p.CloseFigure();
        return p;
    }

    // ── Logic ─────────────────────────────────────────────────────────────────

    private void VerifyToolsOnStartup()
    {
        try { ToolLocator.VerifyAll(); }
        catch (DownloadException)
        {
            MessageBox.Show("Instalação corrompida. Por favor reinstale o VidDrop.",
                "VidDrop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }

    private void OnUrlChanged(object? sender, EventArgs e)
    {
        var url = urlBox.Text.Trim();
        ShowPlatformBadge(url);
        _debounce?.Dispose();
        _debounce = new System.Threading.Timer(
            _ => BeginInvoke(TriggerFetch), null, 800, Timeout.Infinite);
    }

    private void ShowPlatformBadge(string url)
    {
        var (name, color) = DetectPlatform(url);
        if (name is null) { platformLabel.Visible = false; return; }
        platformLabel.Text      = $"▸  {name}";
        platformLabel.ForeColor = color;
        platformLabel.Visible   = true;
    }

    private async void TriggerFetch()
    {
        var url = urlBox.Text.Trim();
        if (string.IsNullOrEmpty(url)) { ResetPreview(); return; }

        _fetchCts?.Cancel();
        _fetchCts = new CancellationTokenSource();
        var ct = _fetchCts.Token;

        titleLabel.Text = "Carregando...";
        thumbBox.Image  = null;

        var meta = await _fetcher.FetchAsync(url, ct);
        if (ct.IsCancellationRequested) return;

        if (meta is null)
        {
            titleLabel.Text = "Preview indisponível — pode tentar baixar mesmo assim.";
        }
        else
        {
            titleLabel.Text = meta.Title;
            thumbBox.Image  = meta.Thumbnail;
        }
    }

    private void ResetPreview()
    {
        titleLabel.Text          = "Cole uma URL acima";
        thumbBox.Image           = null;
        platformLabel.Visible    = false;
    }

    private void OnFormatChanged(object? sender, EventArgs e)
    {
        bool mp3 = formatCombo.SelectedIndex == 1;
        qualityLabel.Visible = !mp3;
        qualityCombo.Visible = !mp3;
        bitrateLabel.Visible = mp3;
        bitrateCombo.Visible = mp3;
    }

    private async void OnActionClick(object? sender, EventArgs e)
    {
        if (_isDownloading) { _coordinator.Cancel(); return; }

        var url = urlBox.Text.Trim();
        if (string.IsNullOrEmpty(url)) return;

        bool mp3 = formatCombo.SelectedIndex == 1;

        var quality = qualityCombo.SelectedIndex switch
        {
            1 => QualityLevel.P1080,
            2 => QualityLevel.P720,
            3 => QualityLevel.P480,
            4 => QualityLevel.P360,
            _ => QualityLevel.Best,
        };

        var bitrate = bitrateCombo.SelectedIndex switch
        {
            0 => Mp3Bitrate.K128,
            2 => Mp3Bitrate.K320,
            _ => Mp3Bitrate.K192,
        };

        var opts = new DownloadOptions
        {
            Url     = url,
            Format  = mp3 ? MediaFormat.Mp3 : MediaFormat.Mp4,
            Quality = quality,
            Bitrate = bitrate,
        };

        SetState(AppState.Downloading);
        try
        {
            await _coordinator.StartAsync(opts, new Progress<DownloadProgress>(OnProgress));
            SetState(AppState.Done, mp3 ? "MP3" : "MP4");
        }
        catch (DownloadException ex) when (ex.Category == ErrorCategory.Cancelled)
        {
            SetState(AppState.Idle, "Download cancelado.");
        }
        catch (DownloadException ex)
        {
            SetState(AppState.Idle, $"Erro: {ex.Message}");
        }
        catch (Exception ex)
        {
            SetState(AppState.Idle, $"Erro inesperado: {ex.Message}");
        }
    }

    private void OnProgress(DownloadProgress p)
    {
        progressBar.Value = (int)Math.Clamp(p.Percent, 0, 100);

        string speed = string.IsNullOrWhiteSpace(p.Speed) || p.Speed == "N/A" ? "" : $"  {p.Speed}";
        string eta   = string.IsNullOrWhiteSpace(p.Eta)   || p.Eta   == "N/A" ? "" : $"  ETA {p.Eta}";

        statusLabel.Text = p.Stage switch
        {
            DownloadStage.Video      => $"Baixando vídeo...  {p.Percent:F0}%{speed}{eta}",
            DownloadStage.Audio      => $"Baixando áudio...  {p.Percent:F0}%{speed}{eta}",
            DownloadStage.Converting => "Finalizando...",
            _                        => $"Baixando...  {p.Percent:F0}%{speed}",
        };
    }

    private void SetState(AppState state, string? hint = null)
    {
        switch (state)
        {
            case AppState.Downloading:
                _isDownloading       = true;
                actionBtn.Text       = "Cancelar";
                actionBtn.IsDanger   = true;
                progressBar.Visible  = true;
                progressBar.Value    = 0;
                statusLabel.Text     = "Iniciando...";
                break;

            case AppState.Done:
                _isDownloading       = false;
                actionBtn.Text       = "Baixar";
                actionBtn.IsDanger   = false;
                progressBar.Visible  = false;
                progressBar.Value    = 0;
                statusLabel.Text     = $"Pronto! {hint} salvo em Downloads\\VidDrop";
                break;

            case AppState.Idle:
                _isDownloading       = false;
                actionBtn.Text       = "Baixar";
                actionBtn.IsDanger   = false;
                progressBar.Visible  = false;
                progressBar.Value    = 0;
                statusLabel.Text     = hint ?? "Cole uma URL e clique em Baixar";
                break;
        }
    }

    private static (string? name, Color color) DetectPlatform(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return (null, default);
        if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            return ("YouTube", Color.FromArgb(200, 30, 0));
        if (url.Contains("twitter.com") || url.Contains("x.com"))
            return ("Twitter / X", Color.FromArgb(0, 100, 180));
        if (url.Contains("instagram.com"))
            return ("Instagram", Color.FromArgb(160, 30, 160));
        if (url.Contains("facebook.com") || url.Contains("fb.watch"))
            return ("Facebook", Color.FromArgb(20, 90, 210));
        if (url.Contains("tiktok.com"))
            return ("TikTok", Color.FromArgb(0, 170, 140));
        if (url.StartsWith("http://") || url.StartsWith("https://"))
            return ("URL genérica", Color.FromArgb(60, 100, 150));
        return (null, default);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _debounce?.Dispose();
        _fetchCts?.Cancel();
        _coordinator.Cancel();
        base.OnFormClosing(e);
    }

    private enum AppState { Idle, Downloading, Done }
}
