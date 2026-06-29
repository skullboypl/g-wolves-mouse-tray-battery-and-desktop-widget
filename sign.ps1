param(
    [string]$Exe = (Join-Path $PSScriptRoot "FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe"),
    [string]$PfxPath = "",
    [string]$PfxPassword = "",
    [string]$TimestampUrl = "http://timestamp.digicert.com"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $Exe)) {
    Write-Error "Exe not found: $Exe`nRun build.ps1 first."
}

$signtool = Get-Command signtool -ErrorAction SilentlyContinue
if (-not $signtool) {
    Write-Error "signtool not in PATH. Install Windows SDK or run from Developer Command Prompt."
}

Write-Host "Signing: $Exe" -ForegroundColor Cyan

if ($PfxPath) {
    if (-not (Test-Path $PfxPath)) { Write-Error "PFX not found: $PfxPath" }
    & signtool sign /fd SHA256 /f $PfxPath /p $PfxPassword /tr $TimestampUrl $Exe
} else {
    & signtool sign /fd SHA256 /a /tr $TimestampUrl $Exe
}

& signtool verify /pa $Exe
Write-Host "Signed OK." -ForegroundColor Green
