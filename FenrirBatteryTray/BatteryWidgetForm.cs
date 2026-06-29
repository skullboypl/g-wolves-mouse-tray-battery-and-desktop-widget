using FenrirBatteryTray.Protocol;

namespace FenrirBatteryTray;

internal sealed class BatteryWidgetForm : Form
{
    private readonly Label _percentLabel = new();
    private readonly Label _statusLabel = new();

    private bool _draggable = true;
    private bool _dragging;
    private bool _suppressPositionSave;
    private Point _dragStart;
    private Action<Point>? _onPositionChanged;

    public BatteryWidgetForm()
    {
        Text = "Fenrir Battery";
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.FromArgb(28, 28, 32);
        Size = new Size(140, 72);

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 8, 10, 8),
        };

        _percentLabel.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
        _percentLabel.ForeColor = Color.White;
        _percentLabel.AutoSize = false;
        _percentLabel.Dock = DockStyle.Top;
        _percentLabel.Height = 36;
        _percentLabel.TextAlign = ContentAlignment.MiddleCenter;
        _percentLabel.Text = "--%";

        _statusLabel.Font = new Font("Segoe UI", 8.5f);
        _statusLabel.ForeColor = Color.FromArgb(170, 170, 180);
        _statusLabel.AutoSize = false;
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.TextAlign = ContentAlignment.TopCenter;
        _statusLabel.Text = "Connecting…";

        panel.Controls.Add(_statusLabel);
        panel.Controls.Add(_percentLabel);
        Controls.Add(panel);

        EnableDrag(this);
        LocationChanged += (_, _) =>
        {
            if (!_dragging && !_suppressPositionSave)
                _onPositionChanged?.Invoke(Location);
        };
    }

    public void SetPositionChangedHandler(Action<Point>? handler) => _onPositionChanged = handler;

    public void ApplyPosition(Point location)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyPosition(location));
            return;
        }

        _suppressPositionSave = true;
        Location = location;
        _suppressPositionSave = false;
    }

    public void ResetToDefaultPosition() => ApplyPosition(AppSettings.DefaultWidgetLocation(Size));

    public void SetDraggable(bool draggable)
    {
        _draggable = draggable;
        Cursor = draggable ? Cursors.SizeAll : Cursors.Default;
    }

    public void UpdateReading(BatteryReading? reading, string? error = null)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => UpdateReading(reading, error));
            return;
        }

        if (error is not null)
        {
            _percentLabel.Text = "--%";
            _statusLabel.Text = error;
            return;
        }

        if (reading is null)
        {
            _percentLabel.Text = "--%";
            _statusLabel.Text = "No data";
            return;
        }

        _percentLabel.Text = $"{reading.Value.Percent}%";
        _statusLabel.Text = reading.Value.Status switch
        {
            BatteryStatus.Charging => "Charging",
            BatteryStatus.Full => "Full",
            BatteryStatus.Discharging => "On battery",
            _ => "G-Wolves mouse",
        };
    }

    private void EnableDrag(Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (!_draggable || e.Button != MouseButtons.Left)
                return;

            _dragging = true;
            _dragStart = control.PointToScreen(e.Location);
            _dragStart = PointToClient(_dragStart);
        };

        control.MouseMove += (_, e) =>
        {
            if (!_draggable || !_dragging || e.Button != MouseButtons.Left)
                return;

            var screen = control.PointToScreen(e.Location);
            var client = PointToClient(screen);
            Location = new Point(
                Location.X + client.X - _dragStart.X,
                Location.Y + client.Y - _dragStart.Y);
            _dragStart = client;
        };

        control.MouseUp += (_, e) =>
        {
            if (e.Button != MouseButtons.Left || !_dragging)
                return;

            _dragging = false;
            _onPositionChanged?.Invoke(Location);
        };

        foreach (Control child in control.Controls)
            EnableDrag(child);
    }
}
