# Fenrir Battery Tray

A lightweight Windows system tray app (and optional desktop widget) that shows the battery level of G-Wolves mice over native HID - no browser, no WebHID.

## Features

- **Tray icon** with live battery level and tooltip (charging / on battery / full)
- **Three tray display modes**: battery icon, percent only, or battery + percent (number overlaid on the battery with a drop shadow)
- **Adjustable tray icon** - separate **font size** (60–400%) and **icon size** (60–400%) sliders
- **Optional desktop widget** - layered, always-on-top, smooth native drag, position is remembered
- **Independent widget opacity** - separate **background opacity** and **font opacity** sliders, blended against the desktop (per-pixel alpha)
- **Adjustable widget size** (50–250%)
- **Settings window** (right-click the tray icon) with live preview
- **Native HID** via [HidSharp](https://www.nuget.org/packages/HidSharp) - same protocol as the WebHID driver
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

- **Tray icon** - display mode (*Battery icon* / *Percent only* / *Battery + percent*), **Font size**, **Icon size**
- **Desktop widget** - show/hide, allow/lock dragging, position (X, Y), **Reset position**, **Use current**, **Size**, **Background opacity**, **Font opacity**
- **Refresh interval** - how often to poll the battery (15–300 seconds)

Defaults: *Percent only*, font size 230%, icon size 109%, refresh 120 s.

Settings are stored in `%AppData%\FenrirBatteryTray\settings.json`. Moving the widget saves its position automatically. All sliders preview live.

## Build Release (.exe)

```powershell
.\build.ps1
```

Output:

`FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe`

Single file, self-contained.

## Code Signing

The published `.exe` can be signed with your own code-signing certificate using
`signtool` (SHA-256 + an RFC-3161 timestamp). Signing is left to the user and is
not part of this repository.

## How it works

1. Scans HID devices by G-Wolves VID/PID
2. Opens the feature-report interface (64 bytes) - same protocol as the WebHID driver
3. Reads battery with fallbacks: legacy cmd `143` → feature cmd `131` → output report cmd `4`
4. Polls on a configurable interval + manual refresh from the menu

## Other models

Defaults to Fenrir Max wireless (`PID 0x3717`). For another model, change `FenrirMaxWirelessPid` in `FenrirBatteryTray/Protocol/GwolvesConstants.cs`.

## License

MIT
