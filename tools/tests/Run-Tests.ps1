$ErrorActionPreference = 'Stop'
$failed = 0
foreach ($t in Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.Tests.ps1' | Sort-Object Name) {
    Write-Host "== $($t.Name)"
    pwsh -NoProfile -File $t.FullName
    if ($LASTEXITCODE -ne 0) { $failed++ }
}
Write-Host '== editor/CtaToybox.Core.Tests'
dotnet run --project (Join-Path $PSScriptRoot '../../editor/CtaToybox.Core.Tests') -c Release
if ($LASTEXITCODE -ne 0) { $failed++ }
Write-Host '== editor/CtaToybox.App (build)'
dotnet build (Join-Path $PSScriptRoot '../../editor/CtaToybox.App') -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) { $failed++ }
if ($failed -gt 0) { Write-Host "$failed test file(s) failed" -ForegroundColor Red; exit 1 }
Write-Host 'all test files passed'
