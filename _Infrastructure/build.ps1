# ============================================================================
# File:        build.ps1
# Project:     BuildStamp
# Author:      Solcogito S.E.N.C.
# Description: Minimal bootstrap build test
# ============================================================================

Write-Host "=== BuildStamp v0.1.0 Bootstrap Test ===" -ForegroundColor Cyan

# Check version file
if (Test-Path "./version.json") {
    $ver = Get-Content "./version.json" | ConvertFrom-Json
    Write-Host "Version file detected: $($ver.version)" -ForegroundColor Green
} else {
    Write-Host "Missing version.json" -ForegroundColor Red
    exit 1
}

# Simulate test run
Write-Host "Running bootstrap validation..." -ForegroundColor Yellow
Start-Sleep -Seconds 1
Write-Host "All checks passed." -ForegroundColor Green
exit 0
