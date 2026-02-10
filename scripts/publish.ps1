#!/usr/bin/env pwsh
# publish.ps1 — Builds front-end + back-end into a single deployable package

param(
    [string]$OutputDir = "./publish"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
if (-not $root) { $root = Get-Location }

Write-Host "=== 1. Building front-end ===" -ForegroundColor Cyan
Push-Location "$root/front-end"
npm ci
npx vite build
Pop-Location

Write-Host "=== 2. Publishing back-end ===" -ForegroundColor Cyan
dotnet publish "$root/back-end/src/Icon.Api/Icon.Api.csproj" `
    -c Release `
    -o "$root/$OutputDir"

Write-Host "=== 3. Copying front-end into wwwroot ===" -ForegroundColor Cyan
$wwwroot = "$root/$OutputDir/wwwroot"
if (-not (Test-Path $wwwroot)) { New-Item -ItemType Directory -Path $wwwroot | Out-Null }
Copy-Item -Path "$root/front-end/dist/*" -Destination $wwwroot -Recurse -Force

Write-Host "=== Done! Output: $root/$OutputDir ===" -ForegroundColor Green
Write-Host "Run with: dotnet Icon.Api.dll"
