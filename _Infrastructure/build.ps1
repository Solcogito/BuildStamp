# ============================================================================
# File:        build.ps1
# Project:     Solcogito.BuildStamp
# Version:     0.8.13
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

# Paths
$CliProject = "src/BuildStamp.Cli/BuildStamp.Cli.csproj"
$ExpectedOutput = "Builds/BuildInfo.cs"   # <= THIS IS WHERE THE CLI WRITES
$OutputDir = "Builds"

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

# Ensure Builds/ exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Remove previous BuildInfo.cs
if (Test-Path $ExpectedOutput) {
    Remove-Item $ExpectedOutput -Force
}

# Run the CLI to generate BuildInfo.cs
dotnet run --project $CliProject --configuration $Configuration -- --emit BuildInfo

if ($LASTEXITCODE -ne 0) {
    Write-Error "CLI failed to generate BuildInfo.cs"
    exit 1
}

# Validate generation
if (!(Test-Path $ExpectedOutput)) {
    Write-Error "❌ BuildInfo.cs not found after generation."
    exit 1
}

Write-Host "✔ BuildInfo.cs generated here: $ExpectedOutput"
Write-Host "=== DONE: BuildStamp CI Verification ==="
