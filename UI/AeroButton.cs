using System.Drawing.Drawing2D;

namespace VidDrop.UI;

public class AeroButton : Button
{
    public bool IsDanger { get; set; }

    private bool _hovered;
    private bool _pressed;

    public AeroButton()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        ForeColor = Color.White;
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _pressed = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        const int r = 10;

        (Color top, Color mid, Color bot) = GetColors();

        using var path = RoundedPath(rect, r);

        // Base gradient
        using var bg = new LinearGradientBrush(
            new Point(0, 0), new Point(0, Height), top, bot);
        g.FillPath(bg, path);

        // Strong gloss — covers top 45%
        int glossH = (int)(Height * 0.45f);
        var glossRect = new Rectangle(1, 1, Width - 3, glossH);
        if (glossRect.Height > 2)
        {
            using var gp = RoundedPath(glossRect, r - 1);
            using var gb = new LinearGradientBrush(
                new Point(0, glossRect.Top), new Point(0, glossRect.Bottom),
                Color.FromArgb(200, 255, 255, 255),
                Color.FromArgb(30, 255, 255, 255));
            g.FillPath(gb, gp);
        }

        // Inner top-edge highlight (1px bright line)
        using var topHighlight = new Pen(Color.FromArgb(160, 255, 255, 255), 1f);
        g.DrawLine(topHighlight, r, 1, Width - r - 1, 1);

        // Outer border
        using var border = new Pen(IsDanger
            ? Color.FromArgb(180, 120, 30, 0)
            : Color.FromArgb(180, 0, 70, 140), 1f);
        g.DrawPath(border, path);

        // Text with subtle shadow
        var shadow = new Rectangle(ClientRectangle.X + 1, ClientRectangle.Y + 1,
                                   ClientRectangle.Width, ClientRectangle.Height);
        TextRenderer.DrawText(g, Text, Font, shadow,
            Color.FromArgb(80, 0, 0, 0),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        TextRenderer.DrawText(g, Text, Font, ClientRectangle,
            Enabled ? Color.White : Color.FromArgb(160, 200, 210),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
    }

    private (Color top, Color mid, Color bot) GetColors()
    {
        if (!Enabled)
            return (Color.FromArgb(185, 200, 215), Color.FromArgb(170, 188, 205), Color.FromArgb(155, 175, 195));

        if (IsDanger)
        {
            return _pressed
                ? (Color.FromArgb(170, 45, 10), Color.FromArgb(145, 35, 5), Color.FromArgb(120, 25, 0))
                : _hovered
                    ? (Color.FromArgb(255, 105, 45), Color.FromArgb(230, 80, 25), Color.FromArgb(200, 55, 10))
                    : (Color.FromArgb(240, 88, 30), Color.FromArgb(215, 68, 18), Color.FromArgb(185, 48, 8));
        }

        return _pressed
            ? (Color.FromArgb(15, 110, 185), Color.FromArgb(10, 90, 160), Color.FromArgb(5, 70, 135))
            : _hovered
                ? (Color.FromArgb(70, 195, 255), Color.FromArgb(30, 160, 230), Color.FromArgb(0, 120, 205))
                : (Color.FromArgb(50, 178, 248), Color.FromArgb(15, 148, 225), Color.FromArgb(0, 108, 195));
    }

    private static GraphicsPath RoundedPath(Rectangle rect, int r)
    {
        int d = r * 2;
        var p = new GraphicsPath();
        p.AddArc(rect.X, rect.Y, d, d, 180, 90);
        p.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        p.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        p.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}
