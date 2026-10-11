. "$PSScriptRoot/TestKit.ps1"

# 작은 바닐라와 모드를 만들어 파이프라인 전체와 비교 스크립트를 함께 돌린다
function Put([string]$root, [string]$rel, [string]$text) {
    $p = Join-Path $root ($rel -replace '/', '\')
    [IO.Directory]::CreateDirectory((Split-Path -Parent $p)) | Out-Null
    [IO.File]::WriteAllText($p, $text)
}

$van = New-TestDir; $mod = New-TestDir
$vanWork = New-TestDir; $vanData = New-TestDir; $modWork = New-TestDir; $modsRoot = New-TestDir; $modData = Join-Path $modsRoot 'm1'

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

Put $mod 'set/stuff/gun/bad' '{from "pattern gun" {x 1}))'                                  # 리뷰 I4: 괄호가 남는 파일도 유닛으로 실려야 한다
Put $mod 'entity/+inventory/z/z.def' '{actor {x 1}}'                                        # 리뷰 I3: 범위 밖 폴더의 정의 파일
Add-Type -AssemblyName System.IO.Compression.FileSystem                                      # 리뷰 I7: 모드 안에서 pak과 낱개 파일이 같은 경로
$zs = [IO.File]::Create((Join-Path $mod 'x.pak')); $za = [IO.Compression.ZipArchive]::new($zs, [IO.Compression.ZipArchiveMode]::Create)
$st = $za.CreateEntry('set/breed/mp/ger/c.set').Open(); $bb = [Text.Encoding]::ASCII.GetBytes('{breed {skin "pak"}}'); $st.Write($bb, 0, $bb.Length); $st.Dispose(); $za.Dispose(); $zs.Dispose()

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
Assert-Equal 8 $files.Count 'every mod index row has a relation'
Assert-Equal 'in-mod-collision' (@($files | Where-Object Status -eq 'collision')[0].Relation) 'second copy of a path inside the mod is not counted as a new file'
Assert-Equal 4 @($files | Where-Object Relation -eq 'new').Count 'new files are counted once per path'

$keys = @(Import-Csv (Join-Path $modData 'diff-keys.csv'))
Assert-Equal @('/breed/newkey', '/from', '/from/x') @($keys.Path | Sort-Object) 'only keys absent from vanilla are listed (the broken file contributes its keys too)'

$units = @(Import-Csv (Join-Path $modData 'diff-units.csv') | Sort-Object Id)
Assert-Equal @('entity/-vehicle/ger/n/n.def', 'set/breed/mp/ger/a.set', 'set/breed/mp/ger/c.set', 'set/stuff/gun/bad') @($units.Id) 'new and changed units, identical one left out, broken file included'
Assert-Equal @('new', 'changed', 'new', 'new') @($units.Relation) 'unit relations'
Assert-Equal @('1', '0') @($units[1].RowsAdded, $units[1].RowsRemoved) 'changed unit row delta'

# 모드의 유닛이 바닐라 파일을 include해서 펼쳐졌는지
$summary = @(Import-Csv (Join-Path $modData 'resolve-summary.csv') | Where-Object VirtualPath -eq 'entity/-vehicle/ger/n/n.def')
Assert-Equal '0' $summary[0].UnresolvedCount 'mod unit resolves include from vanilla work dir'
Assert-Equal '3' $summary[0].ResolvedRows 'game_entity + props + mass'

$gz = [IO.StreamReader]::new([IO.Compression.GZipStream]::new([IO.File]::OpenRead((Join-Path $modData 'units-weapon-ammo.jsonl.gz')), [IO.Compression.CompressionMode]::Decompress))
$badLine = @($gz.ReadToEnd() -split "`n" | Where-Object { $_.Contains('"Id":"set/stuff/gun/bad"') })[0]; $gz.Dispose()
Assert-Equal $true ($badLine.Contains('"ParseErrors":["')) 'catalog line of a broken file carries its parse errors'

& "$PSScriptRoot/../Get-ModSummary.ps1" -VanillaDataDir $vanData -ModsDataDir $modsRoot -ModIds @('m1') *> $null
$sum = Get-Content -Raw (Join-Path $modData 'summary.json') | ConvertFrom-Json
Assert-Equal 1 $sum.OutOfScopeDefinitions.'entity/+inventory' 'definition files outside the analyzed folders are reported'
Assert-Equal 4 $sum.FilesByRelation.new 'summary counts new files once per path'

Complete-Tests
