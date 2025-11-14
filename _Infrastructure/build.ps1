# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.8.3
# Author:      Solcogito S.E.N.C.
# Description: CI validation and BuildInfo generation script
# ============================================================================

param(
    [string] $Configuration = "Release"
)

Write-Host "=== Solcogito BuildStamp CI Verification ==="

# Always run relative to the script location
$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot   = Resolve-Path "$ScriptRoot/.."

Set-Location $RepoRoot
Write-Host "RepoRoot = $RepoRoot"

# -----------------------------------------------------------------------------------------
# 1. RESTORE
# -----------------------------------------------------------------------------------------
Write-Host "`n[1/3] Restoring..."
dotnet restore BuildStamp.sln
if ($LASTEXITCODE -ne 0) { exit 1 }

# -----------------------------------------------------------------------------------------
# 2. BUILD
# -----------------------------------------------------------------------------------------
Write-Host "`n[2/3] Building..."
dotnet build BuildStamp.sln -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { exit 1 }

# -----------------------------------------------------------------------------------------
# 3. GENERATE BUILDINFO
# -----------------------------------------------------------------------------------------
Write-Host "`n[3/3] Generating BuildInfo..."

# Location of output
$OutputPath = "src/BuildStamp.Cli/BuildInfo.cs"

# Remove any stale file
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Force
}

# Generate file (BuildStamp.Cli Program.cs calls BuildStamp.Core)
dotnet run --project src/BuildStamp.Cli/BuildStamp.Cli.csproj `
           --configuration $Configuration `
           -- --emit BuildInfo

if ($LASTEXITCODE -ne 0) {
    Write-Error "CLI failed to generate BuildInfo.cs"
    exit 1
}

# Validate that BuildInfo.cs was created
if (!(Test-Path $OutputPath)) {
    Write-Error "❌ BuildInfo.cs not found after generation."
    exit 1
}

Write-Host "✔ BuildInfo.cs generated successfully"
Write-Host "=== DONE: BuildStamp CI Verification ==="
