$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "FenrirBatteryTray\FenrirBatteryTray.csproj"

Write-Host "Building FenrirBatteryTray (Release, win-x64, single-file)..." -ForegroundColor Cyan

dotnet publish $project `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true

$exe = Join-Path $PSScriptRoot "FenrirBatteryTray\bin\Release\net8.0-windows\win-x64\publish\FenrirBatteryTray.exe"

if (Test-Path $exe) {
    $size = [math]::Round((Get-Item $exe).Length / 1MB, 1)
    Write-Host ""
    Write-Host "OK: $exe ($size MB)" -ForegroundColor Green
    Write-Host "Sign with: signtool sign /fd SHA256 /a `"$exe`"" -ForegroundColor DarkGray
} else {
    Write-Error "Build failed - exe not found."
}
