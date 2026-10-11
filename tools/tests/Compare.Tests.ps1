. "$PSScriptRoot/TestKit.ps1"

# 작은 바닐라와 모드를 만들어 파이프라인 전체와 비교 스크립트를 함께 돌린다
function Put([string]$root, [string]$rel, [string]$text) {
    $p = Join-Path $root ($rel -replace '/', '\')
    [IO.Directory]::CreateDirectory((Split-Path -Parent $p)) | Out-Null
    [IO.File]::WriteAllText($p, $text)
}

$van = New-TestDir; $mod = New-TestDir
$vanWork = New-TestDir; $vanData = New-TestDir; $modWork = New-TestDir; $modData = New-TestDir

Put $van 'set/breed/mp/ger/a.set' '{breed {skin "s1"} {nationality ger}}'
Put $van 'set/breed/mp/ger/b.set' '{breed {skin "s2"}}'
Put $van 'properties/base.ext' '{props "p"}'
Put $van 'entity/-vehicle/ger/t/t.def' '{game_entity (include "/properties/base.ext") {mass 10}}'
[IO.File]::WriteAllBytes((Join-Path $van 'entity\-vehicle\ger\t\t.ply'), [byte[]](1, 2, 3))

Put $mod 'set/breed/mp/ger/a.set' '{breed {skin "s1"} {nationality ger} {newkey 5}}'      # 바뀜: 행 1개 추가
Put $mod 'set/breed/mp/ger/b.set' '{breed {skin "s2"}}'                                    # 같음
Put $mod 'set/breed/mp/ger/c.set' '{breed {skin "s3"}}'                                    # 새 유닛
Put $mod 'entity/-vehicle/ger/n/n.def' '{game_entity (include "/properties/base.ext") {mass 20}}'   # 바닐라 파일을 include
[IO.Directory]::CreateDirectory((Join-Path $mod 'entity\-vehicle\ger\t')) | Out-Null
[IO.File]::WriteAllBytes((Join-Path $mod 'entity\-vehicle\ger\t\t.ply'), [byte[]](9, 9))   # 바이너리 덮어쓰기

& "$PSScriptRoot/../Invoke-Pipeline.ps1" -SourceRoot $van -WorkDir $vanWork -DataDir $vanData *> $null
& "$PSScriptRoot/../Invoke-Pipeline.ps1" -SourceRoot $mod -WorkDir $modWork -DataDir $modData -FallbackWorkDir @($vanWork) -NameDataDir @($vanData) *> $null
& "$PSScriptRoot/../Compare-ModToVanilla.ps1" -VanillaDataDir $vanData -ModDataDir $modData *> $null

$files = @(Import-Csv (Join-Path $modData 'diff-files.csv'))
function Rel([string]$vp) { @($files | Where-Object VirtualPath -eq $vp)[0].Relation }
Assert-Equal 'override-changed' (Rel 'set/breed/mp/ger/a.set') 'changed text file'
Assert-Equal 'override-identical' (Rel 'set/breed/mp/ger/b.set') 'identical text file'
Assert-Equal 'new' (Rel 'set/breed/mp/ger/c.set') 'new text file'
# 내용 해시가 없는 파일(바이너리, 범위 밖)은 같은지 다른지 판정하지 않는다
Assert-Equal 'override-unhashed' (Rel 'entity/-vehicle/ger/t/t.ply') 'override without a content hash is not judged same or different'
Assert-Equal 5 $files.Count 'every mod file has a relation'

$keys = @(Import-Csv (Join-Path $modData 'diff-keys.csv'))
Assert-Equal @('/breed/newkey') @($keys.Path) 'only the key absent from vanilla is listed'

$units = @(Import-Csv (Join-Path $modData 'diff-units.csv') | Sort-Object Id)
Assert-Equal @('entity/-vehicle/ger/n/n.def', 'set/breed/mp/ger/a.set', 'set/breed/mp/ger/c.set') @($units.Id) 'new and changed units, identical one left out'
Assert-Equal @('new', 'changed', 'new') @($units.Relation) 'unit relations'
Assert-Equal @('1', '0') @($units[1].RowsAdded, $units[1].RowsRemoved) 'changed unit row delta'

# 모드의 유닛이 바닐라 파일을 include해서 펼쳐졌는지
$summary = @(Import-Csv (Join-Path $modData 'resolve-summary.csv') | Where-Object VirtualPath -eq 'entity/-vehicle/ger/n/n.def')
Assert-Equal '0' $summary[0].UnresolvedCount 'mod unit resolves include from vanilla work dir'
Assert-Equal '3' $summary[0].ResolvedRows 'game_entity + props + mass'

Complete-Tests
