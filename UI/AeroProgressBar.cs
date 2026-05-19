using System.Drawing.Drawing2D;

namespace VidDrop.UI;

public class AeroProgressBar : Panel
{
    private int _value;

    public int Maximum { get; set; } = 100;

    public int Value
    {
        get => _value;
        set { _value = Math.Clamp(value, 0, Maximum); Invalidate(); }
    }

    public AeroProgressBar()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        const int r = 6;

        // Track
        using var trackPath = RoundedPath(rect, r);
        using var trackBrush = new LinearGradientBrush(
            new Point(0, 0), new Point(0, Height),
            Color.FromArgb(195, 225, 245),
            Color.FromArgb(165, 205, 230));
        g.FillPath(trackBrush, trackPath);

        // Track inner shadow (inset feel)
        var insetRect = new Rectangle(1, 1, Width - 3, Height / 2);
        using var insetBrush = new SolidBrush(Color.FromArgb(20, 0, 60, 120));
        g.FillRectangle(insetBrush, insetRect);

        using var trackBorder = new Pen(Color.FromArgb(120, 160, 200), 1f);
        g.DrawPath(trackBorder, trackPath);

        if (_value <= 0 || Maximum <= 0) return;

        int fillW = Math.Max(r * 2 + 2, (int)((Width - 2.0) * _value / Maximum));
        var fillRect = new Rectangle(1, 1, fillW, Height - 3);

        using var fillPath = RoundedPath(fillRect, r - 1);

        // Aqua gradient fill
        using var fillBrush = new LinearGradientBrush(
            new Point(0, 1), new Point(0, Height - 2),
            Color.FromArgb(80, 200, 255),
            Color.FromArgb(0, 125, 210));
        g.FillPath(fillBrush, fillPath);

        // Gloss on fill — top 45%
        int glossH = (int)((Height - 4) * 0.45f);
        if (glossH > 0 && fillW > 4)
        {
            var glossRect = new Rectangle(2, 2, fillW - 2, glossH);
            using var gb = new LinearGradientBrush(
                glossRect,
                Color.FromArgb(180, 255, 255, 255),
                Color.FromArgb(15, 255, 255, 255),
                LinearGradientMode.Vertical);
            g.FillRectangle(gb, glossRect);
        }

        // Fill shimmer line on top edge
        using var shimmer = new Pen(Color.FromArgb(130, 255, 255, 255), 1f);
        g.DrawLine(shimmer, fillRect.X + r, fillRect.Y, fillRect.Right - r, fillRect.Y);
    }

    private static GraphicsPath RoundedPath(Rectangle rect, int r)
    {
        int d = r * 2;
        var path = new GraphicsPath();
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
