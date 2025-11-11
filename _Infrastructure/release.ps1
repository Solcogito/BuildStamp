# ======================================================================
# File:        release.ps1
# Description: Packages BuildStamp for manual release distribution.
# ======================================================================

param([string]$Version = "0.8.0")

Write-Host "Packaging BuildStamp v$Version..." -ForegroundColor Cyan

dotnet publish ./src/BuildStamp.Cli/BuildStamp.Cli.csproj -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./Builds/BuildStamp-$Version.zip -Force

Write-Host "[OK] Package ready: ./Builds/BuildStamp-$Version.zip" -ForegroundColor Green
