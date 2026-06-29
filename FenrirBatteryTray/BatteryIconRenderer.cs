namespace FenrirBatteryTray;

internal static class BatteryIconRenderer
{
    public static Icon Create(int percent, bool charging, TrayDisplayMode mode)
    {
        return mode switch
        {
            TrayDisplayMode.BatteryIcon => CreateBatteryOnly(percent, charging),
            TrayDisplayMode.Percent => CreatePercentOnly(percent, charging),
            _ => CreateBatteryWithPercent(percent, charging),
        };
    }

    private static Color LevelColor(int percent, bool charging)
    {
        if (charging)
            return Color.FromArgb(90, 170, 255);

        return percent switch
        {
            <= 15 => Color.FromArgb(220, 80, 70),
            <= 30 => Color.FromArgb(230, 150, 60),
            _ => Color.FromArgb(70, 190, 110),
        };
    }

    private static Icon CreateBatteryOnly(int percent, bool charging)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        var fill = LevelColor(percent, charging);
        using var outline = new Pen(Color.FromArgb(220, 220, 220), 1.5f);
        using var brush = new SolidBrush(fill);

        var body = new Rectangle(6, 8, 20, 14);
        g.DrawRectangle(outline, body);
        g.FillRectangle(brush, 8, 10, Math.Max(1, (int)(16 * percent / 100.0)), 10);
        g.FillRectangle(brush, 27, 12, 3, 6);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static Icon CreatePercentOnly(int percent, bool charging)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        var fill = LevelColor(percent, charging);
        using var bg = new SolidBrush(Color.FromArgb(36, 36, 42));
        using var brush = new SolidBrush(fill);
        g.FillEllipse(bg, 2, 2, 28, 28);
        g.DrawEllipse(new Pen(fill, 2f), 3, 3, 26, 26);

        var text = percent.ToString();
        using var font = new Font("Segoe UI", percent >= 100 ? 9f : 11f, FontStyle.Bold);
        var textSize = g.MeasureString(text, font);
        g.DrawString(
            text,
            font,
            brush,
            (size - textSize.Width) / 2f,
            (size - textSize.Height) / 2f - 1);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static Icon CreateBatteryWithPercent(int percent, bool charging)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        var fill = LevelColor(percent, charging);
        using var outline = new Pen(Color.FromArgb(220, 220, 220), 1.5f);
        using var brush = new SolidBrush(fill);
        using var textBrush = new SolidBrush(Color.White);

        var body = new Rectangle(4, 8, 22, 14);
        g.DrawRectangle(outline, body);
        g.FillRectangle(brush, 6, 10, Math.Max(1, (int)(18 * percent / 100.0)), 10);
        g.FillRectangle(brush, 27, 12, 3, 6);

        var text = percent.ToString();
        using var font = new Font("Segoe UI", 7f, FontStyle.Bold);
        var textSize = g.MeasureString(text, font);
        g.DrawString(
            text,
            font,
            textBrush,
            (size - textSize.Width) / 2f,
            size - textSize.Height - 1);

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
