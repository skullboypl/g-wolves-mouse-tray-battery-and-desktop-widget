# Fenrir Battery Tray

A lightweight Windows system tray app (and optional desktop widget) that shows the battery level of G-Wolves mice over native HID — no browser, no WebHID.

## Features

- **Tray icon** with live battery level and tooltip (charging / on battery / full)
- **Three tray display modes**: battery icon, percent only, or battery + percent
- **Optional desktop widget** — draggable, always-on-top, position is remembered
- **Settings window** (right-click the tray icon): tray display mode, widget visibility, drag lock, manual position, reset position, and refresh interval
- **Native HID** via [HidSharp](https://www.nuget.org/packages/HidSharp) — same protocol as the WebHID driver
- **Single-file `.exe`**, self-contained (no .NET install required on the target machine)

## Requirements

- Windows 10/11 (x64)
- .NET 8 SDK (to build)
- A connected G-Wolves mouse (e.g. Fenrir Max wireless dongle: VID `0x33E4`, PID `0x3717`)

## Run (dev)

```powershell
cd FenrirBatteryTray
dotnet run
```

An icon appears next to the clock. Double-click = refresh. Right-click for the menu:
**Refresh now**, **Show desktop widget**, **Settings…**, **Exit**.

## Settings

Right-click the tray icon → **Settings…**

- **Tray icon** — choose: *Battery icon*, *Percent only*, or *Battery + percent*
- **Desktop widget** — show/hide, allow/lock dragging, set position (X, Y), **Reset position**, **Use current**
- **Refresh interval** — how often to poll the battery (15–300 seconds)

Settings are stored in `%AppData%\FenrirBatteryTray\settings.json`. Moving the widget saves its position automatically.

## Build Release (.exe)

```powershell
.\build.ps1
```

Output:

`FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe`

Single file, self-contained.

## EV Code Signing

After building, with a certificate in the Windows certificate store:

```powershell
.\sign.ps1
```

Or with a `.pfx` file:

```powershell
.\sign.ps1 -PfxPath "C:\path\to\cert.pfx" -PfxPassword "password"
```

`sign.ps1` runs `signtool sign /fd SHA256 /tr <timestamp> ...` and then verifies the signature.

## How it works

1. Scans HID devices by G-Wolves VID/PID
2. Opens the feature-report interface (64 bytes) — same protocol as the WebHID driver
3. Reads battery with fallbacks: legacy cmd `143` → feature cmd `131` → output report cmd `4`
4. Polls on a configurable interval + manual refresh from the menu

## Other models

Defaults to Fenrir Max wireless (`PID 0x3717`). For another model, change `FenrirMaxWirelessPid` in `FenrirBatteryTray/Protocol/GwolvesConstants.cs`.

## License

MIT
