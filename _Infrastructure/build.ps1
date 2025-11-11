# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.6.0
# Description: Verification script for Embedded Metadata API integration.
# ============================================================================

Set-Location $PSScriptRoot/..

Write-Host "=== BuildStamp Verification (v0.6.0) ===" -ForegroundColor Cyan

# Configuration
$cliProject = "./src/BuildStamp.Cli/BuildStamp.Cli.csproj"
$configPath = "./buildstamp.json"
$sampleOutput = "./Builds/BuildInfo.cs"

# 1. Clean old outputs
if (Test-Path "./Builds") { Remove-Item "./Builds" -Recurse -Force }
New-Item -ItemType Directory -Path "./Builds" | Out-Null

# 2. Generate metadata file
Write-Host "[1/3] Generating BuildInfo..." -ForegroundColor Gray
dotnet run --project $cliProject -- $configPath
if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] CLI exited with code $LASTEXITCODE" -ForegroundColor Red; exit 1 }

# 3. Validate output exists
if (Test-Path $sampleOutput) {
    Write-Host "[OK] Output file created → $sampleOutput" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Output file missing" -ForegroundColor Red
    exit 1
}

# 4. Verify file contents
$content = Get-Content $sampleOutput -Raw
if ($content -match "public const string Version") {
    Write-Host "[OK] Version constant found in BuildInfo.cs" -ForegroundColor Green
} else {
    Write-Host "[WARN] Version constant missing" -ForegroundColor Yellow
}

# 5. Optional sample build test (if applicable)
if (Test-Path "./src/BuildStamp.Samples/Sample.DotNet/Sample.DotNet.csproj") {
    Write-Host "[3/3] Building sample..." -ForegroundColor Gray
    dotnet build "./src/BuildStamp.Samples/Sample.DotNet/Sample.DotNet.csproj" -c Release
    if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] Sample build failed" -ForegroundColor Red; exit 1 }
    Write-Host "[OK] Sample build passed" -ForegroundColor Green
}

Write-Host "=== BuildStamp v0.6.0 verification complete ===" -ForegroundColor Cyan
exit 0
