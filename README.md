# Fenrir Battery Tray

Prosta aplikacja Windows (system tray) pokazująca poziom baterii myszy G-Wolves przez natywne HID — bez przeglądarki i WebHID.

## Wymagania

- Windows 10/11 (x64)
- .NET 8 SDK (do budowania)
- Mysz G-Wolves podłączona (np. Fenrir Max wireless dongle: VID `0x33E4`, PID `0x3717`)

## Uruchomienie (dev)

```powershell
cd tray-battery\FenrirBatteryTray
dotnet run
```

Ikona pojawi się przy zegarku. Podwójne kliknięcie = odświeżenie. Menu: **Refresh**, **Show desktop widget**, **Exit**.

## Build Release (.exe)

```powershell
cd tray-battery
.\build.ps1
```

Wynik:

`tray-battery\FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe`

Jeden plik, self-contained (~15–20 MB).

## Podpisanie EV Code Signing

Po zbudowaniu, z certyfikatem w magazynie Windows:

```powershell
signtool sign /fd SHA256 /tr http://timestamp.digicert.com /a ".\FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe"
```

Lub z plikiem `.pfx`:

```powershell
signtool sign /fd SHA256 /f "C:\path\to\cert.pfx" /p "haslo" /tr http://timestamp.digicert.com ".\FenrirBatteryTray.exe"
```

## Jak działa

1. Skanuje HID po VID/PID G-Wolves
2. Otwiera interfejs feature report (64 B) — ten sam protokół co `gwolves-driver.ts`
3. Odczyt baterii: legacy cmd `143` → fallback cmd `131` → fallback output report `4`
4. Odświeżanie co 45 s + ręczne z menu

## Inne modele

Domyślnie Fenrir Max wireless (`PID 0x3717`). Inny model: zmień `FenrirMaxWirelessPid` w `Protocol/GwolvesBatteryReader.cs` lub dodaj wybór PID w menu.
# g-wolves-mouse-tray-battery-and-desktop-widget
