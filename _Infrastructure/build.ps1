# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.8.13
# Author:      Solcogito S.E.N.C.
# Description: CI validation and BuildInfo generation script
# ============================================================================
param()

Write-Host "=== Solcogito BuildStamp CI Verification ==="

$RepoRoot = (Get-Location).Path
Write-Host "RepoRoot = $RepoRoot"

Write-Host "`n[1/3] Restoring..."
dotnet restore BuildStamp.sln

Write-Host "`n[2/3] Building..."
dotnet build BuildStamp.sln -c Release --no-restore

Write-Host "`n[3/3] Generating BuildInfo..."
# Path where CLI actually writes BuildInfo.cs
$GeneratedPath = Join-Path $RepoRoot "Builds/BuildInfo.cs"

dotnet run `
  --project "src/BuildStamp.Cli/BuildStamp.Cli.csproj" `
  --configuration Release `
  --output "$(Join-Path $RepoRoot 'Builds')"

if (-not (Test-Path $GeneratedPath)) {
    Write-Error "❌ BuildInfo.cs not found after generation at $GeneratedPath."
    exit 1
}

Write-Host "✔ BuildInfo validated at $GeneratedPath"
