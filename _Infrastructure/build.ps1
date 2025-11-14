# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.8.3
# Author:      Solcogito S.E.N.C.
# Description: CI validation and BuildInfo generation script
# ============================================================================

param(
    [string]$SolutionPath = "./BuildStamp.sln",
    [string]$CliProject = ".src/BuildStamp.Cli/BuildStamp.Cli.csproj"
)

Write-Host "=== Solcogito BuildStamp CI Verification ===" -ForegroundColor Cyan

try {
    if (-not (Test-Path $SolutionPath)) {
        Write-Host "[FAIL] Solution not found at $SolutionPath" -ForegroundColor Red
        exit 1
    }

    Write-Host "[1/3] Restoring..." -ForegroundColor Yellow
    dotnet restore $SolutionPath | Out-Host

    Write-Host "[2/3] Building..." -ForegroundColor Yellow
    dotnet build $SolutionPath -c Release --no-restore | Out-Host

    Write-Host "[3/3] Generating BuildInfo..." -ForegroundColor Yellow
    dotnet run --project $CliProject -- --format cs --out ./Builds/BuildInfo.cs | Out-Host

    if (Test-Path "./Builds/BuildInfo.cs") {
        Write-Host "✅ BuildInfo generated successfully." -ForegroundColor Green
        Write-Host "[CI PASS]" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ BuildInfo.cs not found after generation." -ForegroundColor Red
        Write-Host "[CI FAIL]" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "❌ Exception: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "[CI FAIL]" -ForegroundColor Red
    exit 1
}
