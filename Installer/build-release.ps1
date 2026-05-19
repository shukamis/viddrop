# VidDrop — release build + Velopack packaging
# Usage: .\Installer\build-release.ps1 -Version 1.0.1 [-GithubToken $env:GITHUB_TOKEN] [-Publish]
#
# -Version   : semver version to tag this release (e.g. 1.0.0, 1.1.0)
# -GithubToken: personal access token with repo scope — only needed for -Publish
# -Publish   : upload release to GitHub (requires -GithubToken)

param(
    [Parameter(Mandatory)][string]$Version,
    [string]$GithubToken = $env:GITHUB_TOKEN,
    [switch]$Publish
)

$ErrorActionPreference = "Stop"
$dotnet = "C:\Program Files\dotnet\dotnet.exe"
$vpk    = "$env:USERPROFILE\.dotnet\tools\vpk.exe"
$root   = Split-Path $PSScriptRoot -Parent
$pub    = "$root\bin\Release\net8.0-windows\win-x64\publish"
$dist   = "$root\dist"

Write-Host "`n==> dotnet publish v$Version" -ForegroundColor Cyan
& $dotnet publish "$root\VidDrop.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

Write-Host "`n==> vpk pack v$Version" -ForegroundColor Cyan
& $vpk pack `
    --packId    VidDrop `
    --packVersion $Version `
    --packDir   $pub `
    --mainExe   VidDrop.exe `
    --outputDir $dist

if ($Publish) {
    if (-not $GithubToken) {
        Write-Error "Pass -GithubToken or set `$env:GITHUB_TOKEN"
    }
    Write-Host "`n==> vpk upload github v$Version" -ForegroundColor Cyan
    & $vpk upload github `
        --repoUrl    https://github.com/shukamis/viddrop `
        --publish `
        --releaseName "v$Version" `
        --tag         "v$Version" `
        --token       $GithubToken `
        --outputDir   $dist
    Write-Host "`nRelease v$Version publicada em https://github.com/shukamis/viddrop/releases" -ForegroundColor Green
} else {
    Write-Host "`nPacote gerado em $dist" -ForegroundColor Green
    Write-Host "Para publicar no GitHub: .\Installer\build-release.ps1 -Version $Version -Publish" -ForegroundColor Yellow
}
