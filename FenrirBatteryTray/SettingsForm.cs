namespace FenrirBatteryTray;

internal sealed class SettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly BatteryWidgetForm _widget;
    private readonly Action<AppSettings> _onApply;

    private readonly RadioButton _trayBattery = new() { Text = "Battery icon", AutoSize = true };
    private readonly RadioButton _trayPercent = new() { Text = "Percent only", AutoSize = true };
    private readonly RadioButton _trayBoth = new() { Text = "Battery + percent", AutoSize = true };

    private readonly CheckBox _widgetVisible = new() { Text = "Show desktop widget", AutoSize = true };
    private readonly CheckBox _widgetDraggable = new() { Text = "Allow dragging widget", AutoSize = true };

    private readonly NumericUpDown _posX = new() { Minimum = -32768, Maximum = 32767, Width = 90 };
    private readonly NumericUpDown _posY = new() { Minimum = -32768, Maximum = 32767, Width = 90 };
    private readonly NumericUpDown _pollInterval = new() { Minimum = 15, Maximum = 300, Width = 90, Increment = 15 };

    public SettingsForm(AppSettings settings, BatteryWidgetForm widget, Action<AppSettings> onApply)
    {
        _settings = settings;
        _widget = widget;
        _onApply = onApply;

        Text = "Fenrir Battery — Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(380, 360);
        Font = new Font("Segoe UI", 9f);

        var trayGroup = new GroupBox
        {
            Text = "Tray icon",
            Location = new Point(16, 12),
            Size = new Size(348, 108),
        };

        _trayBattery.Location = new Point(14, 26);
        _trayPercent.Location = new Point(14, 50);
        _trayBoth.Location = new Point(14, 74);
        trayGroup.Controls.AddRange([_trayBattery, _trayPercent, _trayBoth]);

        var widgetGroup = new GroupBox
        {
            Text = "Desktop widget",
            Location = new Point(16, 128),
            Size = new Size(348, 160),
        };

        _widgetVisible.Location = new Point(14, 24);
        _widgetDraggable.Location = new Point(14, 48);

        var posLabel = new Label { Text = "Position (X, Y):", AutoSize = true, Location = new Point(14, 80) };
        _posX.Location = new Point(120, 76);
        _posY.Location = new Point(220, 76);

        var resetBtn = new Button
        {
            Text = "Reset position",
            Location = new Point(14, 112),
            Size = new Size(110, 28),
        };
        resetBtn.Click += (_, _) =>
        {
            var def = AppSettings.DefaultWidgetLocation(_widget.Size);
            _posX.Value = def.X;
            _posY.Value = def.Y;
            _widget.ApplyPosition(def);
        };

        var pickBtn = new Button
        {
            Text = "Use current",
            Location = new Point(132, 112),
            Size = new Size(110, 28),
        };
        pickBtn.Click += (_, _) =>
        {
            if (_widget.Visible)
            {
                _posX.Value = _widget.Location.X;
                _posY.Value = _widget.Location.Y;
            }
        };

        widgetGroup.Controls.AddRange([
            _widgetVisible, _widgetDraggable, posLabel, _posX, _posY, resetBtn, pickBtn,
        ]);

        var pollLabel = new Label
        {
            Text = "Refresh interval (sec):",
            AutoSize = true,
            Location = new Point(16, 300),
        };
        _pollInterval.Location = new Point(170, 296);

        var okBtn = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(188, 322),
            Size = new Size(84, 28),
        };
        okBtn.Click += (_, _) => SaveAndApply(close: true);

        var applyBtn = new Button
        {
            Text = "Apply",
            Location = new Point(100, 322),
            Size = new Size(84, 28),
        };
        applyBtn.Click += (_, _) => SaveAndApply(close: false);

        var cancelBtn = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(276, 322),
            Size = new Size(84, 28),
        };

        Controls.AddRange([trayGroup, widgetGroup, pollLabel, _pollInterval, applyBtn, okBtn, cancelBtn]);

        AcceptButton = okBtn;
        CancelButton = cancelBtn;

        LoadFromSettings();
    }

    private void LoadFromSettings()
    {
        switch (_settings.TrayDisplay)
        {
            case TrayDisplayMode.BatteryIcon:
                _trayBattery.Checked = true;
                break;
            case TrayDisplayMode.Percent:
                _trayPercent.Checked = true;
                break;
            default:
                _trayBoth.Checked = true;
                break;
        }

        _widgetVisible.Checked = _settings.WidgetVisible;
        _widgetDraggable.Checked = _settings.WidgetDraggable;
        _pollInterval.Value = Math.Clamp(_settings.PollIntervalSeconds, 15, 300);

        var pos = _settings.ResolveWidgetLocation(_widget.Size);
        _posX.Value = pos.X;
        _posY.Value = pos.Y;
    }

    private void SaveAndApply(bool close)
    {
        _settings.TrayDisplay = _trayBattery.Checked
            ? TrayDisplayMode.BatteryIcon
            : _trayPercent.Checked
                ? TrayDisplayMode.Percent
                : TrayDisplayMode.BatteryWithPercent;

        _settings.WidgetVisible = _widgetVisible.Checked;
        _settings.WidgetDraggable = _widgetDraggable.Checked;
        _settings.WidgetX = (int)_posX.Value;
        _settings.WidgetY = (int)_posY.Value;
        _settings.PollIntervalSeconds = (int)_pollInterval.Value;

        _settings.Save();
        _onApply(_settings);

        if (close)
            Close();
    }
}
