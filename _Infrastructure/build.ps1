# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Description: Automated multi-format test for BuildStamp v0.4.0
# ============================================================================

Write-Host "=== BuildStamp Multi-Format Test ===" -ForegroundColor Cyan

$Formats = @("json","text","md","cs")

foreach ($fmt in $Formats) {
    $out = "./Builds/buildinfo.$fmt"
    dotnet run --project ./src/BuildStamp -- --format $fmt --out $out

    if (Test-Path $out) {
        Write-Host "[OK] $fmt file generated" -ForegroundColor Green
    } else {
        Write-Host "[FAIL] $fmt file missing" -ForegroundColor Red
        exit 1
    }
}

Write-Host "All formats verified successfully." -ForegroundColor Green
exit 0
