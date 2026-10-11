param(
    [Parameter(Mandatory)][string]$SourceRoot,
    [Parameter(Mandatory)][string]$WorkDir,
    [Parameter(Mandatory)][string]$DataDir,
    [string[]]$FallbackWorkDir = @(),
    [string[]]$NameDataDir = @()
)
$ErrorActionPreference = 'Stop'
# 배열 인자(-FallbackWorkDir, -NameDataDir)가 그대로 전달되도록 같은 프로세스에서 호출한다.
# 각 스크립트는 실패하면 예외를 던진다($ErrorActionPreference = 'Stop').
function Step([string]$name, [scriptblock]$action) {
    Write-Host "=== $name" -ForegroundColor Cyan
    $sw = [Diagnostics.Stopwatch]::StartNew()
    & $action
    Write-Host ("=== $name done in {0:n0}s" -f $sw.Elapsed.TotalSeconds)
}
Step 'extract' { & "$PSScriptRoot/Export-PakText.ps1" -SourceRoot $SourceRoot -OutDir $WorkDir -DataDir $DataDir }
Step 'keys' { & "$PSScriptRoot/Build-KeyInventory.ps1" -WorkDir $WorkDir -DataDir $DataDir }
# 카탈로그 단계가 펼치기, 펼친 키 집계, 참조 색인까지 함께 만든다
Step 'catalog' { & "$PSScriptRoot/Build-UnitCatalog.ps1" -WorkDir $WorkDir -DataDir $DataDir -FallbackWorkDir $FallbackWorkDir -NameDataDir $NameDataDir }
