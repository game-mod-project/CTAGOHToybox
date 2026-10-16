$ErrorActionPreference = 'Stop'
$failed = 0
foreach ($t in Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.Tests.ps1' | Sort-Object Name) {
    Write-Host "== $($t.Name)"
    pwsh -NoProfile -File $t.FullName
    if ($LASTEXITCODE -ne 0) { $failed++ }
}
# 편집기 테스트는 xUnit으로 돈다(P7, 2026-10-17). 묶음마다 테스트 하나이고 실패하면 그 묶음의 FAIL 줄을 보인다.
# 단언 수까지 보려면 dotnet run --project editor/CtaToybox.Core.Tests -c Release
Write-Host '== editor/CtaToybox.Core.XunitTests'
dotnet test (Join-Path $PSScriptRoot '../../editor/CtaToybox.Core.XunitTests') -c Release --nologo
if ($LASTEXITCODE -ne 0) { $failed++ }
Write-Host '== editor/CtaToybox.App (build)'
dotnet build (Join-Path $PSScriptRoot '../../editor/CtaToybox.App') -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) { $failed++ }
if ($failed -gt 0) { Write-Host "$failed test file(s) failed" -ForegroundColor Red; exit 1 }
Write-Host 'all test files passed'
