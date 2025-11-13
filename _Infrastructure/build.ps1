# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.8.3
# Author:      Solcogito S.E.N.C.
# Description: CI validation and BuildInfo generation script.
# ============================================================================

param(
    [string]$ProjectPath = "./src/BuildStamp.sln",
    [string]$CliProject = "./src/BuildStamp.Cli/BuildStamp.Cli.csproj"
)

Write-Host "=== BuildStamp Verification (CI Mode) ===" -ForegroundColor Cyan

try {
    if (-not (Test-Path $ProjectPath)) {
        Write-Host "[FAIL] Solution file not found at $ProjectPath" -ForegroundColor Red
        exit 1
    }

    Write-Host "[1/3] Restoring solution..." -ForegroundColor Yellow
    dotnet restore $ProjectPath | Out-Host

    Write-Host "[2/3] Building solution..." -ForegroundColor Yellow
    dotnet build $ProjectPath -c Release --no-restore | Out-Host

    Write-Host "[3/3] Generating BuildInfo..." -ForegroundColor Yellow
    dotnet run --project $CliProject -- --format cs --out ./Builds/BuildInfo.cs | Out-Host

    if (Test-Path "./Builds/BuildInfo.cs") {
        Write-Host "[OK] BuildInfo generated successfully." -ForegroundColor Green
        Write-Host "[CI PASS]" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "[FAIL] BuildInfo.cs not found after generation." -ForegroundColor Red
        Write-Host "[CI FAIL]" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "[EXCEPTION] $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "[CI FAIL]" -ForegroundColor Red
    exit 1
}
