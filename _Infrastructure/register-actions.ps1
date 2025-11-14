# =====================================================================
# File:        register-actions.ps1
# Project:     Solcogito.BuildStamp
# Description: Forces GitHub to index new workflows and triggers re-run
# =====================================================================

param(
    [string]$Repo = "Solcogito/BuildStamp",
    [string]$Branch = "main",
    [string]$Token = $env:GITHUB_TOKEN
)

if (-not $Token) {
    Write-Host "[INFO] GitHub token not found in environment. Enter it manually:" -ForegroundColor Yellow
    $Token = Read-Host "GitHub Personal Access Token (repo/workflow scope)"
}

Write-Host "=== Checking GitHub Actions indexing for $Repo ===" -ForegroundColor Cyan

# 1. List workflows via REST API
$headers = @{ Authorization = "Bearer $Token"; Accept = "application/vnd.github+json" }
$apiUrl = "https://api.github.com/repos/$Repo/actions/workflows"
$response = Invoke-RestMethod -Uri $apiUrl -Headers $headers -ErrorAction SilentlyContinue

if ($null -eq $response.workflows) {
    Write-Host "[WARN] No workflows indexed yet. Forcing re-registration..." -ForegroundColor Yellow
    Write-Host "[INFO] Committing a whitespace change to .github/workflows/release.yml" -ForegroundColor DarkGray

    # 2. Force workflow re-index by touching file
    (Get-Content ".github/workflows/release.yml") + "`n" | Set-Content ".github/workflows/release.yml"

    git add .github/workflows/release.yml
    git commit -m "ci: force workflow re-registration"
    git push origin $Branch

    Write-Host "[OK] Workflow re-index pushed to GitHub." -ForegroundColor Green
}
else {
    Write-Host "[OK] Workflows detected on GitHub:" -ForegroundColor Green
    $response.workflows | ForEach-Object {
        Write-Host " - $($_.name) [$($_.state)] (id: $($_.id))"
    }
}

# 3. Optional: manually trigger run (for release.yml)
$workflow = ($response.workflows | Where-Object { $_.name -eq "BuildStamp Release" })
if ($workflow) {
    Write-Host "[INFO] Manually triggering workflow 'BuildStamp Release'..." -ForegroundColor Cyan
    $triggerUrl = "https://api.github.com/repos/$Repo/actions/workflows/$($workflow.id)/dispatches"
    $body = @{ ref = $Branch } | ConvertTo-Json
    Invoke-RestMethod -Uri $triggerUrl -Method Post -Headers $headers -Body $body
    Write-Host "[OK] Dispatch triggered on branch '$Branch'." -ForegroundColor Green
}
else {
    Write-Host "[WARN] Workflow 'BuildStamp Release' not found. It may take ~1 min to register." -ForegroundColor Yellow
}

Write-Host "`n=== Done ==="
